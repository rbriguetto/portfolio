using System.Net;
using Xunit;
using Infraestructure.Data;
using SmartApps.Jobs.Application.Commands;
using SmartApps.Jobs.Application.ViewModels;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartApps.Jobs.IntegrationTests.Api.Controllers;

public class JobControllerTests : IClassFixture<IntegrationTestsApplicationFactory>
{
    private readonly IntegrationTestsApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public JobControllerTests(IntegrationTestsApplicationFactory factory)
    {
        _factory = factory;

        _jsonOptions = new JsonSerializerOptions() { 
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }


    [Fact()]
    public async Task CreateJobApiShouldReturnCreatedStatusCode()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/Job/CreateJob", 
            new CreateJobCommand() { Handler = Guid.NewGuid().ToString(), 
            Payload = Guid.NewGuid().ToString()} );
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact()]
    public async Task CreateJobApiShouldSaveJobOnRepository()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/Job/CreateJob", 
            new CreateJobCommand() { Handler = Guid.NewGuid().ToString(), 
            Payload = Guid.NewGuid().ToString()} );
        response.EnsureSuccessStatusCode();
        var createJobResponse = await response.Content.ReadFromJsonAsync<CreateJobResponse>();
        var serviceProvider = _factory.Services.CreateScope().ServiceProvider;
        var jobRepository = serviceProvider.GetRequiredService<IJobRepository>();
        var job = jobRepository.GetJobById(createJobResponse?.JobId ?? string.Empty);
        Assert.NotNull(job);
    }

    [Fact()]
    public async Task CreateJobShouldReturnBadRequestWhenErrorOnCreatingJob()
    {
        var client = _factory.CreateClient();
        var invalidJobRequest = new CreateJobCommand() { Handler = string.Empty, Payload = string.Empty };
        var response = await client.PostAsJsonAsync("/api/Job/CreateJob", invalidJobRequest); 
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact()]
    public async Task GetJobByIdShouldReturnTheJobWhenItExists()
    {
        var serviceProvider = _factory.Services.CreateScope().ServiceProvider;
        var transactionManager = serviceProvider.GetRequiredService<ITransactionManager>();
        var jobRepository = serviceProvider.GetRequiredService<IJobRepository>();
        var job = new Job(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        await jobRepository.CreateJob(job);
        transactionManager.RequireNew();
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/Job/GetJobById?id={job.JobId}");
        response.EnsureSuccessStatusCode();
        var responseJob = await response.Content.ReadFromJsonAsync<JobViewModel>(_jsonOptions);
        Assert.Equal(job.JobId, responseJob?.JobId);
    }

    [Fact()]
    public async Task GetJobByIdShouldReturnNotFoundWhenJobIdDoNotExists()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/Job/GetJobById?id={Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}