namespace Infraestructure.Data;

public class ConnectionStringOptions
{
    public static readonly string Section = "ConnectionString";
    
    public string DriverName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string TablePrefix { get; set; } = string.Empty;
}