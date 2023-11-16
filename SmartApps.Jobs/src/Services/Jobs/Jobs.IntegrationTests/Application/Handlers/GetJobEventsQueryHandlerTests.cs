using MediatR;
using Xunit;
using SmartApps.Jobs.Application.Query;
using SmartApps.Jobs.Application.Handlers;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;
using Moq;
using Elastic.CommonSchema;

namespace SmartApps.Jobs.IntegrationTests.Application.Handlers;

public class GetJobEventsQueryHandlerTests : IClassFixture<IntegrationTestsApplicationFactory>
{
    private readonly IntegrationTestsApplicationFactory _factory;

    public GetJobEventsQueryHandlerTests(IntegrationTestsApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact()]
    public async Task ShouldReturnItemsFromEventRepository()
    {
        var expectedItems = new[]{
            new JobEvent() { Message = Guid.NewGuid().ToString() },
            new JobEvent() { Message = Guid.NewGuid().ToString() },
            new JobEvent() { Message = Guid.NewGuid().ToString() }
        };
        var eventRepositoryMock = new Mock<IJobEventRepository>();
        eventRepositoryMock.Setup(x => x.GetEventsByJobId(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedItems);
        var sut = new GetJobEventsQueryHandler(eventRepositoryMock.Object);
        var evts = await sut.Handle(new GetJobEventsQuery() { JobId = Guid.NewGuid().ToString() }, CancellationToken.None);
        Assert.Equal(expectedItems, evts);
    }

    [Fact()]
    public async Task ShouldReturnEmptyWhenDoNotExistItems()
    {
        var expectedItems = Enumerable.Empty<JobEvent>();
        var eventRepositoryMock = new Mock<IJobEventRepository>();
        eventRepositoryMock.Setup(x => x.GetEventsByJobId(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedItems);
        var sut = new GetJobEventsQueryHandler(eventRepositoryMock.Object);
        var evts = await sut.Handle(new GetJobEventsQuery() { JobId = Guid.NewGuid().ToString() }, CancellationToken.None);
        Assert.Empty(evts);
    }
}