using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using TgBotFramework.UpdatePipeline;

namespace TgBotFramework
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PollingManager<TContext> : BackgroundService, IPollingManager<TContext>
        where TContext : IUpdateContext
    {
        private readonly ILogger<PollingManager<TContext>> _logger;
        private readonly LongPollingOptions _pollingOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ChannelWriter<IUpdateContext> _channel;
        private readonly TelegramBotClient _client;
        

        public PollingManager(
            ILogger<PollingManager<TContext>> logger, 
            LongPollingOptions pollingOptions, 
            IOptions<BotSettings> botOptions, 
            Channel<IUpdateContext> channel,
            IServiceProvider serviceProvider) 
        {
            _logger = logger;
            _pollingOptions = pollingOptions;
            _serviceProvider = serviceProvider;
            _channel = channel.Writer;
            _client = new TelegramBotClient(botOptions.Value.ApiToken, baseUrl: botOptions.Value.BaseUrl);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _client.DeleteWebhookAsync(cancellationToken: cancellationToken);
            if (_pollingOptions.DebugOutput)
            {
                _client.OnApiResponseReceived += ReceiveLogger;
            }

            int messageOffset = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var updates = await _client.GetUpdatesAsync(messageOffset, 0, _pollingOptions.Timeout,
                        _pollingOptions.AllowedUpdates, cancellationToken);

                    foreach (var update in updates)
                    {
                        var updateContext = _serviceProvider.GetService<IUpdateContext>();
                        
                        Debug.Assert(updateContext != null, nameof(updateContext) + " != null");
                        updateContext.Update = update;
                        
                        await _channel.WriteAsync(updateContext, cancellationToken);
                        messageOffset = update.Id + 1;

                        if (_pollingOptions.WaitForResult)
                        {
                            updateContext.Result = new TaskCompletionSource();
                            await updateContext.Result.Task;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while polling in " + nameof(PollingManager<TContext>));
                }
            }
        }



        private async ValueTask ReceiveLogger(ITelegramBotClient client, ApiResponseEventArgs args, CancellationToken token)
        {
            if (args.ApiRequestEventArgs.MethodName == "getUpdates")
            {
                var message = await args.ResponseMessage.Content.ReadAsStringAsync(token);
                JToken jobj = JObject.Parse(message)["result"];
                foreach (var item in jobj)
                {
                    //_logger.LogInformation($"[{item["update_id"]}] Content:\n{{1}}", item);
                    _logger.LogInformation("[{0}] Content:\n{1}", item["update_id"].ToString(), item);
                }
            }

        /*    _logger.LogInformation("Received response, method {0}, code {1} content:\n\t{2}",
                args.ApiRequestEventArgs.MethodName,
                args.ResponseMessage.StatusCode,
                await args.ResponseMessage.Content.ReadAsStringAsync(token));  */
        }
    }

    public interface IPollingManager<in TContext> 
        where TContext : IUpdateContext
    {
    }
}