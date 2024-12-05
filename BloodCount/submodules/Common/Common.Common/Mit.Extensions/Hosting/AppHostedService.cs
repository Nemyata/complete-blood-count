using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Common.Extensions.Hosting;

public class AppHostedService : IHostedService
{
    private IHostApplicationLifetime applicationLifetime;

    public AppHostedService(IHostApplicationLifetime applicationLifetime)
    {
        this.applicationLifetime = applicationLifetime;
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        applicationLifetime.StopApplication();
        return Task.CompletedTask;
    }
    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}