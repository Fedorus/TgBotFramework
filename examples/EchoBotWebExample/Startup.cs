using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EchoBotProject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            
            services.AddBotService<EchoBot, BaseUpdateContext>(x=> x
                .UseLongPolling<PollingManager<BaseUpdateContext>>(new LongPollingOptions())
                .SetPipeline(x=> x
                        .Use<UpdateLogger>()
                        .Use<GlobalExceptionHandler>()
                    )
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

    public class GlobalExceptionHandler : IUpdateHandler<BaseUpdateContext>
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BaseUpdateContext context, UpdateDelegate<BaseUpdateContext> next, CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken);
                _logger.LogInformation("Update {0}, no errors", context.Update.Id);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Update {0}, has errors {1}", context.Update.Id, e);
            }
        }
    }

    public class UpdateLogger : IUpdateHandler<BaseUpdateContext>
    {
        private readonly ILogger<UpdateLogger> _logger;

        public UpdateLogger(ILogger<UpdateLogger> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BaseUpdateContext context, UpdateDelegate<BaseUpdateContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update {0}, no errors", context.Update.Id );
            await next(context, cancellationToken);
        }
    }
}