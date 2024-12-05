using Microsoft.Extensions.DependencyInjection;

namespace Common.ProxyHttp
{
    public static class ProxyHttpServiceCollectionExtensions
    {
        public static void AddProxy(this IServiceCollection services)
        {
            services.AddTransient<ProxyHttpClientHandler>();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddHttpClient(CommonConstants.ProxyClient).ConfigurePrimaryHttpMessageHandler<ProxyHttpClientHandler>();
        }
    }
}
