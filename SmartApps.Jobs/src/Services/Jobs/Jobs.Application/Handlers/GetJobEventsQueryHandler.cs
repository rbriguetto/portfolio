using MediatR;
using SmartApps.Jobs.Application.Query;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.Application.Handlers;

public class GetJobEventsQueryHandler : IRequestHandler<GetJobEventsQuery, IEnumerable<JobEvent>>
{
    private readonly IJobEventRepository _jobEventRepository;

    public GetJobEventsQueryHandler(IJobEventRepository jobEventRepository)
    {
        _jobEventRepository = jobEventRepository; 
    }

    public Task<IEnumerable<JobEvent>> Handle(GetJobEventsQuery request, CancellationToken cancellationToken) =>
        _jobEventRepository.GetEventsByJobId(request.JobId, cancellationToken);
}