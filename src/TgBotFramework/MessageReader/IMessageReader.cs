using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotFramework.MessageReader
{
    public interface IMessageReader<TUpdateHandler, TContext>
        where TContext : IUpdateContext
        where TUpdateHandler : class
    {
        string GetMessage(string jsonPath);
    }
}
