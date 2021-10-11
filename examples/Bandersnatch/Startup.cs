using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Bandersnatch.Bot;
using CommonHandlers;
using EchoBotProject.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types.Enums;
using TgBotFramework;
using TgBotFramework.Data.EF;

namespace Bandersnatch
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
            services.Configure<BotSettings>(Configuration.GetSection(nameof(BandersnatchBot)));
            services.AddBotService<BandersnatchBot, BandersnatchContext>(builder => builder

                .UseLongPolling<PollingManager<BandersnatchContext>>(new LongPollingOptions())

                .UseMiddleware<GlobalExceptionHandler<BandersnatchContext>>()

                .UseEF<BandersnatchContext, BotFrameworkContext >(StateStrategy.PerUser)

                .UseStates(Assembly.GetAssembly(typeof(BandersnatchBot)))
                .UseCommands(Assembly.GetAssembly(typeof(BandersnatchBot)))

                .SetPipeline(pipelineBuilder => pipelineBuilder
                    .MapWhen(On.MyChatMember, addedToChatBranch => addedToChatBranch
                        // add keyboard with callbacks to set up bot 
                        .MapWhen<BotAddedToNewChat>(context =>
                            context.Update.MyChatMember.NewChatMember.User.Username == context.Bot.Username &&
                            context.Update.MyChatMember.NewChatMember.Status is ChatMemberStatus.Member or
                                ChatMemberStatus.Administrator)
                        
                        .MapWhen<BotPromotedToAdmin>(context =>
                            context.Update.MyChatMember.NewChatMember.User.Username == context.Bot.Username &&
                            context.Update.MyChatMember.NewChatMember.Status is ChatMemberStatus.Administrator)
                        
                        .Use<BotWasKickedOrBlocked>()
                    )
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

    public class BotWasKickedOrBlocked : IUpdateHandler<BandersnatchContext>
    {
        public async Task HandleAsync(BandersnatchContext context, UpdateDelegate<BandersnatchContext> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class BotPromotedToAdmin : IUpdateHandler<BandersnatchContext>
    {
        public async Task HandleAsync(BandersnatchContext context, UpdateDelegate<BandersnatchContext> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class BotAddedToNewChat : IUpdateHandler<BandersnatchContext>
    {
        public async Task HandleAsync(BandersnatchContext context, UpdateDelegate<BandersnatchContext> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}