using BloodCount.Web.ServiceCollection;

using BloodCount.Domain.Configuration;
using BloodCount.DomainServices.Implementation;

using BloodCount.DataAccess.Implementation;

using Common.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Http.Features;

using Serilog;



var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
        .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment)
        .Enrich.WithProperty("Version", typeof(Program).Assembly.GetName().Version);
});

var services = builder.Services;
IConfiguration configuration = builder.Configuration;


services.AddCompression();
services.AddSession();
services.ConfigureSection<ConnectionStrings>(configuration);
services.ConfigureSection<FileConfigurations>(configuration);
services.ConfigureSection<PythonConfigurations>(configuration);

services.AddServiceCollection();
services.AddRepositories();
services.AddPythonCalback();


services.AddControllerWithViews();

services.Configure<FormOptions>(options =>
{
    // Set up the upload size limit 256MB
    options.MultipartBoundaryLengthLimit = 256 * 1024 * 1024;
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("APIPolicy");
app.UseSession();

app.MapDefaultControllerRoute();

app.Run();