using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EchoBotProject;
using EchoBotProject.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TgBotFramework;
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
            services.AddLogging();
            services.Configure<BotSettings>(Configuration.GetSection(nameof(EchoBot)));
            services.AddScoped<UpdateLogger>();
            services.AddScoped<GlobalExceptionHandler>();
            services.AddScoped<MessageHandler>();
            
            
            services.AddBotService<EchoBot, BaseUpdateContext>(x=> x
                .UseLongPolling<PollingManager<BaseUpdateContext>>(new LongPollingOptions())
                .UseMiddleware<UpdateLogger>()
                .UseMiddleware<GlobalExceptionHandler>()
                .SetPipeline(x=> x
                        .Use<MessageHandler>()
                    )
                .UseStates(Assembly.GetAssembly(typeof(EchoBot)))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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