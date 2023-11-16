using MediatR;
using Xunit;
using SmartApps.Jobs.Application.Commands;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.IntegrationTests.Application.Handlers;

public class CreateJobCommandHandlerTests : IClassFixture<IntegrationTestsApplicationFactory>
{
    private readonly IntegrationTestsApplicationFactory _factory;

    public CreateJobCommandHandlerTests(IntegrationTestsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact()]
    public async Task JobShouldBeCreatedWhenInputIsValid()
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
        var result = await mediator.Send(command, CancellationToken.None);
        var job = await jobRepository.GetJobById(result.JobId);
        Assert.NotNull(job);
    }

    [Fact()]
    public async Task ShouldReturnPendingJobWhenItExists()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var mediator = sp.GetRequiredService<IMediator>();
        var command = new CreateJobCommand()
        {
            Handler = "TestHandler",
            Payload = "TestPayload",
            UserAgent = "UserAgent"
        };
        var result1 = await mediator.Send(command, CancellationToken.None);
        var result2 = await mediator.Send(command, CancellationToken.None);
        Assert.Equal(result1.JobId, result2.JobId);
    }

    [Fact()]
    public async Task ShouldCreateNewJobWhenInputIsDifferent()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var mediator = sp.GetRequiredService<IMediator>();
        var result1 = await mediator.Send(new CreateJobCommand()
        {
            Handler = "TestHandler",
            Payload = "TestPayload",
            UserAgent = "UserAgent"
        });
        var result2 = await mediator.Send(new CreateJobCommand()
        {
            Handler = "TestHandler",
            Payload = "Another-TestPayload",
            UserAgent = "UserAgent"
        });
        Assert.NotEqual(result1.JobId, result2.JobId);
    }

    [Fact()]
    public async Task ShouldCreateNewJobWhenExistingJobWithSameInputIsCompleted()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var mediator = sp.GetRequiredService<IMediator>();
        var command = new CreateJobCommand() {
            Handler = "TestHandler",
            Payload = "TestPayload",
            UserAgent = "UserAgent"
        };
        var result1 = await mediator.Send(command);
        await mediator.Send(new MarkJobAsFinishedCommand() { IsSuccess = true, JobId = result1.JobId });
        var result2 = await mediator.Send(command);
        Assert.NotEqual(result1.JobId, result2.JobId);
    }
}