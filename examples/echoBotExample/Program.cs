using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EchoBotProject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace echoBotExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
        }
    }

    internal class MyFirstMiddleware : IUpdateHandler<MyContext>
    {
        public async Task HandleAsync(MyContext context, UpdateDelegate<MyContext> next, CancellationToken cancellationToken)
        {
            Console.WriteLine(1);
            await next(context, cancellationToken);
        }
    }
    
    internal class MySecondMiddleware<T> : IUpdateHandler<T> where T : IUpdateContext
    {
        public async Task HandleAsync(T context, UpdateDelegate<T> next, CancellationToken cancellationToken)
        {
            Console.WriteLine(2);
        }
    }

    public class MyContext : BaseUpdateContext
    {
    }
}