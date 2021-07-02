using System;

namespace TgBotFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class StateAttribute : System.Attribute
    {
        public string Stage { get; set; }
    }
}