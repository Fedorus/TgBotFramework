using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using TgBotFramework.Attributes;

namespace TgBotFramework.MessageReader
{
    public class MessageReader<TUpdateHandler, TContext> : IMessageReader<TUpdateHandler, TContext>
        where TContext : IUpdateContext
        where TUpdateHandler : class
    {
        private readonly Assembly _resourceAssembly;
        private readonly string _rootDirectory;
        private readonly string _language;
        private readonly string _stage;
        private readonly Stream _stream;
        private readonly StreamReader _fileReader;

        public MessageReader(string rootDirectory = "Resources", string language = "ru", Assembly resourceAssembly = null)
        {
            _rootDirectory = rootDirectory;
            _language = language;
            _resourceAssembly = resourceAssembly ?? Assembly.GetCallingAssembly();

            var handlerAttribute = typeof(TUpdateHandler).GetCustomAttribute<HandlerAttribute>();
            if (handlerAttribute != null)
            {
                _stage = handlerAttribute.Stage + ".handler";
            }
            else
            {
                var stateAttribute = typeof(TUpdateHandler).GetCustomAttribute<StateAttribute>();
                if (stateAttribute != null)
                {
                    _stage = stateAttribute.Stage + ".stage";
                }
            }

            if (_stage == null)
            {
                throw new InvalidOperationException("The IUpdateHandler class should contain State or Handler attribute");
            }

            var resourcePath = $"{_resourceAssembly.GetName().Name}.{_rootDirectory}.{_language}.{_stage}.json";
            _stream = _resourceAssembly.GetManifestResourceStream(resourcePath);

            if (_stream == null)
            {
                throw new InvalidOperationException(
                    $"Assembly {_resourceAssembly.FullName} doesn't contain EmbeddedResource at path {resourcePath}. Resource file cannot be loaded");
            }

            _fileReader = new StreamReader(_stream);
            MessageFileText = _fileReader.ReadToEnd();
        }

        public string MessageFileText { get; private set; }

        public void Dispose()
        {
            _fileReader.Close();
            _stream.Close();
        }

        public string GetMessage(string jsonPath)
        {
            JObject JsonObject = JsonConvert.DeserializeObject<JObject>(MessageFileText);
            var node = JsonObject.SelectToken(jsonPath);
            if (node == null)
            {
                throw new ArgumentException($"There are no values found by path '{jsonPath}' in stage JSON file '{_stage}'");
            }
            return node.ToString();
        }
    }
}
