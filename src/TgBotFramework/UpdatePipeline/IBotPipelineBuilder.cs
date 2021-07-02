using System;
using Microsoft.Extensions.Logging;

namespace TgBotFramework.UpdatePipeline
{
    public interface IBotPipelineBuilder<TContext> where TContext : IUpdateContext
    {
        ILogger<IBotPipelineBuilder<TContext>> Logger { get; }
        IBotPipelineBuilder<TContext> Use(Func<UpdateDelegate<TContext>, UpdateDelegate<TContext>> middleware);
        IBotPipelineBuilder<TContext> Use<THandler>() where THandler : IUpdateHandler<TContext>;
        IBotPipelineBuilder<TContext> Use<THandler>(THandler handler) where THandler : IUpdateHandler<TContext>;
        UpdateDelegate<TContext> Build();
    }
}