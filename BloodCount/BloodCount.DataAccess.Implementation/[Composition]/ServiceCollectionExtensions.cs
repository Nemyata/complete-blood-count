using BloodCount.DataAccess.Interfaces;
using BloodCount.DataAccess.Interfaces.Main;
using BloodCount.DataAccess.Interfaces.Python;
using BloodCount.DataAccess.Implementation.Main;
using BloodCount.DataAccess.Implementation.Python;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace BloodCount.DataAccess.Implementation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.TryAddTransient<IConnectionService, ConnectionService>();

        services.TryAddTransient<IFilesRepository, FilesRepository>();
        services.TryAddTransient<ILLMRepository, LLMRepository>();

        return services;
    }

    public static IServiceCollection AddPythonCalback(this IServiceCollection services)
    {
        services.TryAddTransient<IPythonCallback, PythonCallback>();

        return services;
    }
}