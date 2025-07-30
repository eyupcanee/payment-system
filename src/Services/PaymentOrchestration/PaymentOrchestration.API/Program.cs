using PaymentOrchestration.API.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
        .AddEnvironmentVariables()
        .Build())
    .CreateLogger();


try
{
    Log.Information("Payment Orchestration Service starting...");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddInfrastructureServices(builder.Configuration).AddPresentationServices(builder.Configuration);
    builder.Services.AddAuthorization();
    
    builder.WebHost.ConfigureKestrel(options => options.Configure(builder.Configuration.GetSection("Kestrel"!)));
    var kestrelUrl = builder.Configuration.GetValue<string>("Kestrel:Endpoints:Http:Url");
    
    Log.Information("Kestrel Configured Successfully. Service Address Url: {KestrelUrl}", kestrelUrl);

    var app = builder.Build();

    app.UseCustomMiddlewares();

    app.Run();

}
catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design")  // see https://github.com/dotnet/efcore/issues/29923
{
    Log.Fatal(ex, "Payment Orchestration Service start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
