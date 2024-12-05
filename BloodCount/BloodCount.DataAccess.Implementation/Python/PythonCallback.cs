using BloodCount.Domain.Configuration;
using BloodCount.DataAccess.Interfaces.Python;

using Common;

using System.Text;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;



namespace BloodCount.DataAccess.Implementation.Python;
public class PythonCallback(IOptions<PythonConfigurations> options,
                            ILogger<PythonCallback> logger) : IPythonCallback
{
    private readonly PythonConfigurations _configuration = options.Value;
    private readonly ILogger<PythonCallback> _logger = logger;

    public async Task<string> CallPythonAsync(PythonType script, string args)
    {
        try
        {
            _logger.LogInformation("Вызов {type} скрипта", script.GetDisplayName());
            ProcessStartInfo start = new()
            {
                FileName = _configuration.Python,
                Arguments = _configuration.GetArguments(script, args),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using Process? process = Process.Start(start);
            using StreamReader reader = process!.StandardOutput;

            var bytes = default(byte[]);
            using (var memstream = new MemoryStream())
            {
                var buffer = new byte[512];
                var bytesRead = default(int);
                while ((bytesRead = await reader.BaseStream.ReadAsync(buffer)) > 0)
                    await memstream.WriteAsync(buffer.AsMemory(0, bytesRead));
                bytes = memstream.ToArray();
            }

            string result = Encoding.GetEncoding(1251).GetString(bytes);
            _logger.LogInformation("Результат работы скрипта {type}: {result}", script.GetDisplayName(), result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось вызвать {type} скрипт", script.GetDisplayName());
            throw;
        }
    }
}