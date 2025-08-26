using FraudDetection.API.Extensions;
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
    Log.Information("Fraud Detection Service starting...");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddInfrastructureServices(builder.Configuration).AddPresentationServices(builder.Configuration);
    
    var app = builder.Build();

    app.UseCustomMiddlewares();

    app.Run();

}
catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design")  // see https://github.com/dotnet/efcore/issues/29923
{
    Log.Fatal(ex, "Fraud Detection Service start-up failed");
}
finally
{
    Log.CloseAndFlush();
}