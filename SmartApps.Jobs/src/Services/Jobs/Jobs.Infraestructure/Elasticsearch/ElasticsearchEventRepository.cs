using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.Infraestructure.Elasticsearch; 

public class ElasticsearchEventRepository : IJobEventRepository
{
    private readonly ElasticserviceClientProvider _clientProvider;

    public ElasticsearchEventRepository(ElasticserviceClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }
    public async Task<IEnumerable<JobEvent>> GetEventsByJobId(string jobId, CancellationToken cancellationToken = default)
    {
        var request = new SearchRequest("*") 
        {
            From = 0,
            Size = 10,
            Query = new TermQuery("job.id") { Value = jobId }
        };
        var response = await _clientProvider.Client.SearchAsync<dynamic>(request, cancellationToken);
        return response.Documents
            .Where(document => document is not null)
            .Select(document => (JsonElement)document)
            .Select(element => element.ToJobEvent())
            .OrderByDescending(evt => evt.CreateUtc)
            .ToList();
    }
}