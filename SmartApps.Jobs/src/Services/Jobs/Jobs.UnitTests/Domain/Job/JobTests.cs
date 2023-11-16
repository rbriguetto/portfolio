using Xunit;
using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.UnitTests.Domain;

public class JobTests
{
    [Fact()]
    public void JobShouldBeAbleToMoveFromPendingToStartedState()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Pending, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        sut.MarkAsStarted();
        Assert.Equal(JobStatus.Started, sut.Status);
    }

    [Fact()]
    public void JobStartedUtcShouldBeUpdatedWhenMarkedAsStarted()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Pending, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        sut.MarkAsStarted();
        Assert.NotNull(sut.StartedAtUtc);
    }

    [Fact()]
    public void JobFinishedUtcShouldBeUpdatedWhenMarkedAsFinished()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Pending, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        sut.MarkAsStarted();
        sut.MarkAsFinished();
        Assert.NotNull(sut.FinishedAtUtc);
    }

    [Fact()]
    public void WithResponseMethodShouldUpdateResponseField()
    {
        var responseValue = Guid.NewGuid().ToString();
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Pending, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        sut.MarkAsStarted();
        sut.MarkAsFinished().WithResponse(responseValue);
        Assert.Equal(responseValue, sut.Response);
    }

    [Fact()]
    public void MarkAsFinishedMethodShouldUpdateStatusWithSuccess()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Pending, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        sut.MarkAsStarted();
        sut.MarkAsFinished();
        Assert.Equal(JobStatus.Success, sut.Status);
    }

    [Fact()]
    public void MarkAsFinishedMethodShouldUpdateStatusWithError()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Pending, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        sut.MarkAsStarted();
        sut.MarkAsFinished(false);
        Assert.Equal(JobStatus.Error, sut.Status);
    }

    [Fact()]
    public void JobShouldNotBeAbleToMoveFromAStartedToStartedState()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Started, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        Assert.Throws<JobAlreadyStartedException>(() => sut.MarkAsStarted());
    }

    [Fact()]
    public void JobShouldNotBeAbleToMoveFromAFinishedToFinishedState()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Success, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        Assert.Throws<JobAlreadyFinishedException>(() => sut.MarkAsFinished());
    }

    [Fact()]
    public void JobShouldNotBeAbleToMoveFromAErrorToFinishedState()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Error, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        Assert.Throws<JobAlreadyFinishedException>(() => sut.MarkAsFinished());
    }

    [Fact()]
    public void JobShouldThrownExceptionWhenPayloadIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Job("handler", string.Empty));
    }

    [Fact()]
    public void JobShouldThrownExceptionWhenHandlerIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Job(string.Empty, "anypayload"));
    }

    [Fact()]
    public void ShouldCreateUniqueId()
    {
        var sut = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Started, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        Assert.NotEmpty(sut.UniqueId);
    }

    [Fact()]
    public void ShouldCreateSameUniqueIdWhenSameInput()
    {
        var sut1 = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Started, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        var sut2 = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Started, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        Assert.Equal(sut1.UniqueId, sut2.UniqueId);
    }

    [Fact()]
    public void ShouldCreateDifferentUniqueIdWhenInputsAreDifferent()
    {
        var sut1 = new Job(Guid.NewGuid().ToString(), "handler", "payload", JobStatus.Started, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        var sut2 = new Job(Guid.NewGuid().ToString(), "handler", "another-payload", JobStatus.Started, 
            DateTime.UtcNow, null, null, "test", "", Enumerable.Empty<JobEvent>());
        Assert.NotEqual(sut1.UniqueId, sut2.UniqueId);
    }
}