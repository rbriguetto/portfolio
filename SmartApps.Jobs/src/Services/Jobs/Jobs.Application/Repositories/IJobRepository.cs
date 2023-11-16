using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Repositories;

public interface IJobRepository
{
    Task<Job> GetJobWithEventsById(string jobId, CancellationToken cancellationToken = default);
    Task<Job> GetJobById(string jobId, CancellationToken cancellationToken = default);
    Task<Job> GetPendingJobByUniqueId(string uniqueId, CancellationToken cancellationToken = default);
    Task CreateJob(Job job, CancellationToken cancellationToken = default);
    Task UpdateJob(Job job, CancellationToken cancellationToken = default);
}