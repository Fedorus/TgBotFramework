using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EchoBotProject;
using EchoBotProject.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
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
            await Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    IConfiguration config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();
                    
                    services.AddLogging();
                    services.Configure<BotSettings>(config.GetSection(nameof(EchoBot)));
                    services.AddScoped<UpdateLogger>();
                    services.AddScoped<GlobalExceptionHandler>();
                    services.AddScoped<MessageHandler>();


                    services.AddBotService<EchoBot, BaseUpdateContext>(x => x
                        .UseLongPolling<PollingManager<BaseUpdateContext>>(new LongPollingOptions())
                        .UseMiddleware<UpdateLogger>()
                        .UseMiddleware<GlobalExceptionHandler>()
                        .SetPipeline(x => x
                            .Use<MessageHandler>()
                        )
                        .UseStates(Assembly.GetAssembly(typeof(EchoBot)))
                    );

                }).RunConsoleAsync();
        }
    }
}