using System;

namespace TgBotFramework.UpdatePipeline
{
    public static class BuilderExtensions
    {
        public static IBotPipelineBuilder<TContext> UseWhen<TContext>(
            this IBotPipelineBuilder<TContext> builder,
            Predicate<TContext> predicate,
            Action<IBotPipelineBuilder<TContext>> configure
        )where TContext : IUpdateContext
        {
            var branchBuilder = new BotPipelineBuilder<TContext>(builder.Logger);
            configure(branchBuilder);
            UpdateDelegate<TContext> branchDelegate = branchBuilder.Build();

            builder.Use(new UseWhenMiddleware<TContext>(predicate, branchDelegate));

            return builder;
        }

        
        public static IBotPipelineBuilder<TContext> UseWhen<THandler, TContext>(
            this IBotPipelineBuilder<TContext> builder, 
            Predicate<TContext> predicate 
            )
            where THandler : IUpdateHandler<TContext>
            where  TContext : IUpdateContext
        {
            
            var branchDelegate = new BotPipelineBuilder<TContext>(builder.Logger).Use<THandler>().Build();
            builder.Use(new UseWhenMiddleware<TContext>(predicate, branchDelegate));
            
            return builder;
        }

        public static IBotPipelineBuilder<TContext> MapWhen<TContext>( 
            this IBotPipelineBuilder<TContext> builder, 
            Predicate<TContext> predicate,
            Action<IBotPipelineBuilder<TContext>> configure)
            where  TContext : IUpdateContext
        {
            var mapBuilder = new BotPipelineBuilder<TContext>(builder.Logger);
            configure(mapBuilder);
            var mapDelegate = mapBuilder.Build();

            builder.Use(new MapWhenMiddleware<TContext>(predicate, mapDelegate));

            return builder;
        }
        
        public static IBotPipelineBuilder<TContext> MapWhen<THandler, TContext>(
            this IBotPipelineBuilder<TContext> builder, Predicate<TContext> predicate)
            where THandler : IUpdateHandler<TContext>
            where  TContext : IUpdateContext
        {
            var branchDelegate = new BotPipelineBuilder<TContext>(builder.Logger).Use<THandler>().Build();
            
            builder.Use(new MapWhenMiddleware<TContext>(predicate, branchDelegate));
            
            return builder;
        }
    }
}