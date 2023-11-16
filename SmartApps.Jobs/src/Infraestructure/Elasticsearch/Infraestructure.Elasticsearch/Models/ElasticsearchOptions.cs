using Microsoft.Extensions.Configuration;

namespace Infraestructure.Elasticsearch.Extensions;

public class ElasticsearchOptions
{
    public static readonly string Section = "Elasticsearch";

    public string Uri { get; set; } = string.Empty;
    public string IndexName { get; set; } = string.Empty;

    public string Host { get { return ExtractHostFromUri(Uri);}}
    public string Username { get { return ExtractUsernameFromUri(Uri); }}
    public string Password { get { return ExtractPasswordFromUri(Uri); }}
    // public string Port { get { return ExtractPortFromUri(Uri); } }

    public static ElasticsearchOptions FromConfiguration(IConfiguration configuration) => 
        new ElasticsearchOptions() { 
            Uri = configuration[$"{Section}:{nameof(ElasticsearchOptions.Uri)}"] ?? string.Empty,
            IndexName = configuration[$"{Section}:{nameof(ElasticsearchOptions.IndexName)}"] ?? string.Empty
        };
    
    public static string ExtractHostFromUri(string uri) 
    {
        if (string.IsNullOrEmpty(uri))
            return string.Empty;
        var startIndex = uri.LastIndexOf("@") ;
        if (startIndex == -1)
            return string.Empty;
        var hostWithoutProtocol = uri.Substring(startIndex + 1);
        var protocol = ExtractProtocolFromUri(uri);
        return $"{protocol}{hostWithoutProtocol}";
    }
    public static string ExtractProtocolFromUri(string uri) 
    {
        if (string.IsNullOrEmpty(uri)) 
            return string.Empty;
        var separatorIndex = uri.IndexOf(":");
        if (separatorIndex == -1) 
            return string.Empty;
        return $"{uri.Substring(0, uri.IndexOf(":"))}://";
    }

    public static string ExtractUsernameFromUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
            return string.Empty;
        var startIndex = uri.IndexOf("//");
        if (startIndex == -1)
            return string.Empty;
        var endIndex = uri.IndexOf(":", startIndex);
        if (endIndex == -1)
            return string.Empty;
        return uri.Substring(startIndex + 2, endIndex - (startIndex + 2));
    }

    public static string ExtractPasswordFromUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
            return string.Empty;
        var startIndex = uri.IndexOf("//");
        if (startIndex == -1)
            return string.Empty;
        startIndex = uri.IndexOf(":", startIndex);
        if (startIndex == -1)
            return string.Empty;
        var endIndex = uri.LastIndexOf("@");
        if (endIndex == -1)
            return string.Empty;
        var value = uri.Substring(startIndex + 1, endIndex - (startIndex + 1));
        return value;
    }
    
    //http://elastic:123@localhost:9200/
}