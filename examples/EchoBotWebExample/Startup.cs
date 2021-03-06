using System;
using System.Linq;
using System.Reflection;
using CommonHandlers;
using EchoBotProject;
using EchoBotProject.Commands;
using EchoBotProject.Handlers;
using EchoBotProject.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.Data.EF;
using TgBotFramework.UpdatePipeline;

namespace EchoBotWebExample
{
    public class Startup
    {
        IConfiguration Configuration { get; set; }
        public Startup(IConfiguration conf)
        {
            Configuration = conf;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder.
                AddConfiguration(Configuration.GetSection("Logging"))
                .AddConsole()
                .AddFile(o=>o.RootPath = AppContext.BaseDirectory)
            );
            services.Configure<BotSettings>(Configuration.GetSection(nameof(EchoBot)));
            services.AddScoped<UpdateLogger>();
            services.AddScoped<GlobalExceptionHandler<BotExampleContext>>();
            services.AddScoped<StopwatchHandler>();
            services.AddScoped<SimpleUserRequestLimiter>();
            
            // register deps for pipeline
            services.ServicesForExamplePipelineBuilder();

            services.AddDbContext<BotFrameworkContext>(x =>
                x.UseSqlite("Data Source=BotFramework.db",
                    builder => builder.MigrationsAssembly("EchoBotProject")));

            // set your bot and context (inherit from basic one)
            services.AddBotService<EchoBot, BotExampleContext>(x => x
                //select way/settings for getting updates
                .UseLongPolling<PollingManager<BotExampleContext>>(new LongPollingOptions() {DebugOutput = true} )
                // add services that fill your updateContext, handling exceptions, logging updates, etc

                .UseMiddleware<GlobalExceptionHandler<BotExampleContext>>()
                .UseMiddleware<StopwatchHandler>()
                
                //not stateless handler, needs rewriting...
                .UseMiddleware<SimpleUserRequestLimiter>()
                // you may use this approach to logging but be aware that not all update objects can be converted back to json
                // .UseMiddleware<UpdateLogger>()
                
                // if you want to use states... 
                .UseStates(Assembly.GetAssembly(typeof(EchoBot)))
                
                .UseEF<BotExampleContext, ExampleDbContext>(StateStrategy.PerUser)
                // if you don`t wanna setup commands in pipeline
                .UseCommands(Assembly.GetAssembly(typeof(EchoBot)))

                // set update processing pipeline (executes last)
                .SetPipeline(pipelineBuilder => pipelineBuilder
                     // in echo bot project
                    .ExamplePipelineBuilder()
                )
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BotFrameworkContext db)
        {
            //db.Database.EnsureCreated();
            db.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}