using CleanArchitecture.Application.Interfaces;
using Serilog;
using Serilog.Filters;

namespace CleanArchitecture.Infrastructure.ExternalServices.SeriLog;

public class SerilogLogger : ISerilogLogger
{
    private readonly global::Serilog.ILogger _logger;

    public SerilogLogger()
    {
        //Skapar Log-mappen om den inte finns
        if (!Directory.Exists("Logs"))
        {
            Directory.CreateDirectory("Logs");
        }


        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("ApplicationContext", "EventSphere")
            .Enrich.FromLogContext()
            .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.Console()
            .WriteTo.File("Logs/EventSphere-log-.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {ApplicationContext} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
       
    }
    
    public void LogInformation(string message)
    {
        _logger.Information(message);
    }

    public void LogError(string message, Exception ex)
    {
        _logger.Error(message);
    }
}

