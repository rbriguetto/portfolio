using System.Text.Json;
using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Infraestructure.Elasticsearch; 

public static class ElasticsearchJobEventExtensions
{
    public static JobEvent ToJobEvent(this JsonElement element) => new JobEvent() {
        Message = element.GetProperty("message").GetString() ?? string.Empty,
        LogLevel = element.GetProperty("log.level").GetString() ?? string.Empty,
        CreateUtc = Convert.ToDateTime(element.GetProperty("@timestamp").GetString())
    };
}