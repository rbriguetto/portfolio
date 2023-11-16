using MediatR;
using Xunit;
using SmartApps.Jobs.Application.Commands;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.IntegrationTests.Application.Handlers;

public class MarkJobAsStartedCommandHandlerTests : IClassFixture<IntegrationTestsApplicationFactory>
{
    private readonly IntegrationTestsApplicationFactory _factory;

    public MarkJobAsStartedCommandHandlerTests(IntegrationTestsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact()]
    public async Task JobShouldBeMarkedAsStarted()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var jobRepository = sp.GetRequiredService<IJobRepository>();
        var mediator = sp.GetRequiredService<IMediator>();
        var command = new CreateJobCommand()
        {
            Handler = "TestHandler",
            Payload = "TestPayload",
            UserAgent = "UserAgent"
        };
        var createJobResponse = await mediator.Send(command, CancellationToken.None);
        await mediator.Send(
            new MarkJobAsStartedCommand() { JobId = createJobResponse.JobId }, 
                CancellationToken.None);
        var job = await jobRepository.GetJobById(createJobResponse.JobId);
        Assert.Equal(JobStatus.Started, job.Status);
    }

    [Fact()]
    public async Task ShouldThrowJobNotFoundExceptionWhenJobIdIsNotFound()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var jobRepository = sp.GetRequiredService<IJobRepository>();
        var mediator = sp.GetRequiredService<IMediator>();
        await Assert.ThrowsAsync<JobNotFoundException>(() => mediator.Send(new MarkJobAsStartedCommand() { JobId = Guid.NewGuid().ToString()}));
    }
}