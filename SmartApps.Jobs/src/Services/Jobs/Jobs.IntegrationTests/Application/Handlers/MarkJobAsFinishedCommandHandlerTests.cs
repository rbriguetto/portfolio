using MediatR;
using Xunit;
using SmartApps.Jobs.Application.Commands;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.IntegrationTests.Application.Handlers;

public class MarkJobAsFinishedCommandHandlerTests : IClassFixture<IntegrationTestsApplicationFactory>
{
    private readonly IntegrationTestsApplicationFactory _factory;

    public MarkJobAsFinishedCommandHandlerTests(IntegrationTestsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact()]
    public async Task JobShouldBeMarkedAsFinished()
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
            new MarkJobAsFinishedCommand() 
            { 
                JobId = createJobResponse.JobId , 
                IsSuccess = true, 
                Response = "RESPONSEPAYLOAD" 
            }, 
            CancellationToken.None);
        var job = await jobRepository.GetJobById(createJobResponse.JobId);
        Assert.Equal(JobStatus.Success, job.Status);
    }

    [Fact()]
    public async Task ShouldThrowJobNotFoundExceptionWhenJobIdIsNotFound()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var mediator = sp.GetRequiredService<IMediator>();
        await Assert.ThrowsAsync<JobNotFoundException>(() => mediator.Send( new MarkJobAsFinishedCommand() { JobId = Guid.NewGuid().ToString(), 
                IsSuccess = true, 
                Response = "RESPONSEPAYLOAD" 
            }, CancellationToken.None));
    }

    [Fact()]
    public async Task JobShouldBeMarkedAsFinishedWithError()
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
        await mediator.Send(new MarkJobAsFinishedCommand() { JobId = createJobResponse.JobId , IsSuccess = false, 
            Response = "RESPONSEPAYLOAD" }, CancellationToken.None);
        var job = await jobRepository.GetJobById(createJobResponse.JobId);
        Assert.Equal(JobStatus.Error, job.Status);
    }

    [Fact()]
    public async Task ResponseShouldBeUpdatedWhenJobIsFinished()
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
        var markJobAsFinishedCommand = new MarkJobAsFinishedCommand() 
        { 
            JobId = createJobResponse.JobId , 
            IsSuccess = true, 
            Response = "RESPONSEPAYLOAD"
        };
        await mediator.Send(markJobAsFinishedCommand, CancellationToken.None);
        var job = await jobRepository.GetJobById(createJobResponse.JobId);
        Assert.Equal(markJobAsFinishedCommand.Response, job.Response);
    }
}