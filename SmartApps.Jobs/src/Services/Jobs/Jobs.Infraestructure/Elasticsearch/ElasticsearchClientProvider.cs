using Microsoft.Extensions.Options;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Infraestructure.Elasticsearch.Extensions;

namespace SmartApps.Jobs.Infraestructure.Elasticsearch; 

public class ElasticserviceClientProvider
{
    private readonly ElasticsearchOptions _elasticsearchOptions;
    private ElasticsearchClient? _client = null;

    public ElasticserviceClientProvider(IOptions<ElasticsearchOptions> elasticsearchOptions)
    {
        _elasticsearchOptions = elasticsearchOptions.Value;
    }

    public ElasticsearchClient Client { 
        get 
        { 
            if (_client == null) 
            {
                CreateClient(); 
            }
            return _client!;
        }
    }

    private void CreateClient()
    {
        if (_client != null) 
        {
            return;
        }
        var settings = new ElasticsearchClientSettings(new Uri(_elasticsearchOptions.Host))
            .ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true)
            .DisableDirectStreaming(true)
            .Authentication(new BasicAuthentication(_elasticsearchOptions.Username, _elasticsearchOptions.Password));
        settings.DefaultIndex(_elasticsearchOptions.IndexName);
        _client = new ElasticsearchClient(settings);
    }
}