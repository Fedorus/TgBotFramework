using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommonHandlers;
using EchoBotProject;
using EchoBotProject.Commands;
using EchoBotProject.Data.MongoDB;
using EchoBotProject.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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
                    services.AddScoped<GlobalExceptionHandler<BotExampleContext>>();
                    services.AddScoped<MessageHandler>();
                    services.AddScoped<StartCommand>();
                    services.AddScoped<ReturnState>();
                    services.AddScoped<PublicChatEcho>();


                    services.AddBotService<EchoBot, BotExampleContext>(x => x
                        .UseMongoDb(new MongoUrl("mongodb://localhost:27017/EchoBot"))
                        //select way/settings for getting updates
                        .UseLongPolling<PollingManager<BotExampleContext>>(new LongPollingOptions())
                        // add services that fill your updateContext, handling exceptions, logging updates, etc
                        .UseMiddleware<UpdateLogger>()
                        .UseMiddleware<GlobalExceptionHandler<BotExampleContext>>()

                        // if you want to use states... 
                        .UseStates(Assembly.GetAssembly(typeof(EchoBot)))
                        // if you don`t wanna setup commands in pipeline
                        .UseCommands(Assembly.GetAssembly(typeof(EchoBot)))

                        // set update processing pipeline (executes last)
                        .SetPipeline(pipelineBuilder => pipelineBuilder
                            .MapWhen(On.Message, onMessageBuilder => onMessageBuilder
                                    .MapWhen(In.PrivateChat, branch => branch
                                        .UseCommand<StartCommand>("start")
                                        .Use<MessageHandler>()
                                    )
                                    .MapWhen<PublicChatEcho>(x => In.GroupChat(x) || In.SupergroupChat(x))
                                // this was same as, just demonstration how you can combine such statements with your own
                                //.MapWhen<PublicChatNotification>(In.GroupOrSupergroup)
                            )
                        )
                    );

                }).RunConsoleAsync();
        }
    }
}