using Identity.API.Extensions;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Identity Service starting...");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddInfrastructureServices(builder.Configuration).AddPresentationServices(builder.Configuration);
    builder.WebHost.ConfigureKestrel(options => options.Configure(builder.Configuration.GetSection("Kestrel")));
    var kestrelUrl = builder.Configuration["Kestrel:Endpoints:Http:Url"];

    Log.Information("Kestrel Configured Successfully. Service Address Url: {KestrelUrl}", kestrelUrl);


    var app = builder.Build();

    app.UseCustomMiddlewares();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Identity Service start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

