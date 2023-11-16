using MediatR;
using Xunit;
using SmartApps.Jobs.Repositories;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Application.Query;

namespace SmartApps.Jobs.IntegrationTests.Application.Handlers;

public class GetJobStatusQueryHandlerTests : IClassFixture<IntegrationTestsApplicationFactory>
{
    private readonly IntegrationTestsApplicationFactory _factory;

    public GetJobStatusQueryHandlerTests(IntegrationTestsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact()]
    public async Task ShouldReturnJobWhenItExists()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var jobRepository = sp.GetRequiredService<IJobRepository>();
        var mediator = sp.GetRequiredService<IMediator>();
        var job = new Job("handler", "payload");
        await jobRepository.CreateJob(job);
        var result = await mediator.Send(new GetJobByIdQuery() { JobId = job.JobId });
        Assert.Equal(job.JobId, result.JobId);
    }

    [Fact()]
    public async Task ShouldThrowJobNotFoundExceptionWhenJobIdDoNotExists()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var jobRepository = sp.GetRequiredService<IJobRepository>();
        var mediator = sp.GetRequiredService<IMediator>();
        await Assert.ThrowsAsync<JobNotFoundException>(() => mediator.Send(new GetJobByIdQuery() { JobId = Guid.NewGuid().ToString() }));
    }
}