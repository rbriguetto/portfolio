namespace SmartApps.Jobs.Domain;

public class JobEvent
{
    public string Message { get; set; } = string.Empty;
    public string LogLevel { get; set; } = string.Empty;
    public DateTime? CreateUtc {get; set; } = null;
}