using System;
using System.Collections.Generic;

namespace TgBotFramework.UpdatePipeline
{
    public class UpdatePipelineSettings<TContext> where TContext : IUpdateContext
    {
        public List<Type> Middlewares { get; } = new List<Type>();
        public Func<IBotPipelineBuilder<TContext>, IBotPipelineBuilder<TContext>> PipeSettings { get; set; }
        public SortedDictionary<string, Type> States { get; set; } = new ();
        public List<Type> Commands { get; set; } = new List<Type>();
    }
}