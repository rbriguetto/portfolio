using Xunit;

namespace SmartApps.Jobs.IntegrationTests.Infraestructure.Elasticsearch;

public class ElasticsearchEventRepositoryTests : IClassFixture<IntegrationTestsApplicationFactory>
{
    private readonly IntegrationTestsApplicationFactory _factory;

    public ElasticsearchEventRepositoryTests(IntegrationTestsApplicationFactory factory)
    {
        _factory = factory;
    }

   //[Fact()]
   //public async Task GetEventsByIdShouldReturnValues()
   //{
   //    //todo: Melhorar a forma de garantir que o log foi enviado
   //    //e processado pelo Elasticsearch. Em algumas situações o teste
   //    //falha somente pelas mensagens ainda não tiverem sido processadas
   //    //pelo RabbitMq
   //    int expectedCount = 4;
   //    var jobId = "TEST-JOB";
   //    var sp = _factory.Services.CreateScope().ServiceProvider;
   //    var logger = sp.GetRequiredService<ILogger<ElasticsearchEventRepositoryTests>>();
   //    logger.LogError("AAAAAAAAAAA {job_id}", jobId);
   //    logger.LogError("BBBBBBBBB {job_id}", jobId);
   //    logger.LogError("CCCCCCCCCC {job_id}", jobId);
   //    logger.LogError("AAAAAAAAAAA {job_id}", "another-job-id");
   //    logger.LogError("DDDDDDDDD {job_id}", jobId);
   //    await Task.Delay(TimeSpan.FromSeconds(30));
   //    var sut = sp.GetRequiredService<IJobEventRepository>();
   //    var response = await sut.GetEventsByJobId(jobId);
   //    Assert.Equal(expectedCount, response.Count());
   //}
}