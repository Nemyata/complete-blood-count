using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog;

namespace Common.Diagnostics;

public class SerilogToolkit
{
    /// <summary>
    /// Создание временного логера на этап запуска.
    /// Затем его заменит логгер из <see cref="HostBuilderExtensions.UseSerilogCommonConfiguration{TProgram}"/>, настраиваемый позднее.</summary>
    public static ReloadableLogger CreateBootstrapCommonLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();
    }
}