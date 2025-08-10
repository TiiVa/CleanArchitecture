namespace CleanArchitecture.Application.Interfaces;

public interface ISerilogLogger
{
    void LogInformation(string message);
    void LogError(string message, Exception ex);
}