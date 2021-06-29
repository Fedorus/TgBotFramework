using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework
{
    public delegate Task UpdateDelegate<TContext>(TContext context, CancellationToken cancellationToken = default) where TContext : IUpdateContext;
}