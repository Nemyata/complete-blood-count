using System.IO.Compression;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.ResponseCompression;


namespace BloodCount.Web.ServiceCollection;

public static class ServiceCollectionExtensions
{
    private static readonly Regex ConfigRx = new("(Configuration|SettingsOptions|Settings|Options|Parameters)$");

    public static void AddCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });
    }

    public static void AddControllerWithViews(this IServiceCollection services)
    {
        //разрешаем CORS
        services.AddCors(option => option.AddPolicy("APIPolicy", builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }));

        services.AddControllersWithViews();
    }
}