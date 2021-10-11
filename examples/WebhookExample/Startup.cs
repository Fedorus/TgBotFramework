using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommonHandlers;
using EchoBotProject;
using EchoBotProject.Commands;
using EchoBotProject.Handlers;
using EchoBotProject.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using TgBotFramework;
using TgBotFramework.Webhook;

namespace WebhookTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.Configure<BotSettings>(Configuration.GetSection(nameof(EchoBot)));
            services.AddScoped<UpdateLogger>();
            services.AddScoped<GlobalExceptionHandler<BotExampleContext>>();
            services.AddScoped<MessageHandler>();
            services.AddScoped<StartCommand>();
            services.AddScoped<ReturnState>();
            services.AddScoped<PublicChatEcho>();
            services.AddScoped<GameState<BotExampleContext>>();
            services.AddScoped<PlayCommand>();

            //services.AddDbContext<BotFrameworkContext>(x=>x.UseSqlite("Data Source=BotFramework.sqlite"));

            // set your bot and context (inherit from basic one)
            services.AddBotService<EchoBot, BotExampleContext>(x => x
                .UseWebhook(new WebhookSettings(){ WaitForResult   = true})
                //select way/settings for getting updates
                //.UseLongPolling<PollingManager<BotExampleContext>>(new LongPollingOptions())
                // add services that fill your updateContext, handling exceptions, logging updates, etc
                .UseMiddleware<UpdateLogger>()
                .UseMiddleware<GlobalExceptionHandler<BotExampleContext>>()
                
                // if you want to use states... 
                .UseStates(Assembly.GetAssembly(typeof(EchoBot)))
                //.UseEF()
                // if you don`t wanna setup commands in pipeline
                .UseCommands(Assembly.GetAssembly(typeof(EchoBot)))

                // set update processing pipeline (executes last)
                .SetPipeline(pipelineBuilder => pipelineBuilder
                    .MapWhen(On.Message, onMessageBuilder => onMessageBuilder
                        .MapWhen(In.PrivateChat, branch => branch
                            .UseCommand<StartCommand>("start")
                            .Use<MessageHandler>()
                        )
                        .MapWhen<PublicChatEcho>(x=> In.GroupChat(x) || In.SupergroupChat(x))
                        // this was same as, just demonstration how you can combine such statements with your own
                        //.MapWhen<PublicChatNotification>(In.GroupOrSupergroup)
                    )
                )
            );
            
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebhookExample", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebhookExample v1"));
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseTelegramBotWebhook();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}