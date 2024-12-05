using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Extensions.Hosting;

public static class HostExtensions
{
    public static async Task<int> TryRunAsync(this IHost host, CancellationToken token = default(CancellationToken))
    {
        Log.Information("Host is created.");

        try
        {
            Log.Information("Host is starting...");
            await host.RunAsync(token);
            Log.Information("Host is stopped.");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}