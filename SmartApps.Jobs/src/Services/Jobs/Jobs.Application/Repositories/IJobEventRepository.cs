using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Repositories;

public interface IJobEventRepository
{
    Task<IEnumerable<JobEvent>> GetEventsByJobId(string jobId, CancellationToken cancellationToken = default);
}