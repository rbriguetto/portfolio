using MediatR;
using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Application.Query;

public class GetJobEventsQuery: IRequest<IEnumerable<JobEvent>>
{
    public string JobId { get; set; } = string.Empty;
}