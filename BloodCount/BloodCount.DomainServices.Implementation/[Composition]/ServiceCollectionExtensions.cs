using BloodCount.DomainServices.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace BloodCount.DomainServices.Implementation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceCollection(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AppMappingProfile));

        services.TryAddTransient<IAnalysisService, AnalysisService>();

        return services;
    }
}