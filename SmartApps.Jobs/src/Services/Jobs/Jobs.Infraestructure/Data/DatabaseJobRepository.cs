using Infraestructure.Data;
using Infraestructure.Utils.Extensions;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.Infraestructure.Data;

public class DatabaseJobRepository : IJobRepository
{
    private readonly IJobEventRepository _jobEventRepository;
    private readonly IRepository<JobRecord> _jobRepository;
    private readonly ITransactionManager _transactionManager;

    public DatabaseJobRepository(
        IRepository<JobRecord> jobRepository, 
        IJobEventRepository jobEventRepository,
        ITransactionManager transactionManager)
    {
        _jobEventRepository = jobEventRepository;
        _jobRepository = jobRepository;
        _transactionManager = transactionManager;
    }

    public async Task CreateJob(Job job, CancellationToken cancellationToken = default) {
        var jobRecord = new JobRecord() {
            JobId = job.JobId,
            Handler = job.Handler,
            Payload = job.Payload,
            CreateUtc = job.CreatedUtc,
            StartedAtUtc = job.StartedAtUtc,
            FinishedAtUtc = job.FinishedAtUtc,
            UserAgent = job.UserAgent,
            Response = job.Response,
            UniqueId = job.UniqueId,
            Status = JobStatus.Pending
        };
        await _jobRepository.CreateAsync(jobRecord, cancellationToken);
        _transactionManager.RequireNew();
    }

    public async Task UpdateJob(Job job, CancellationToken cancellationToken = default) {
        var jobRecord = await _jobRepository.GetAsync(record => record.JobId == job.JobId, cancellationToken);
        if (jobRecord == null)
        {
            throw new JobNotFoundException();
        }
        jobRecord.Status = job.Status;
        jobRecord.Response = job.Response;
        jobRecord.StartedAtUtc = job.StartedAtUtc ;
        jobRecord.FinishedAtUtc = job.FinishedAtUtc;
        jobRecord.Payload = job.Payload;
        jobRecord.UserAgent = job.UserAgent;
        await _jobRepository.UpdateAsync(jobRecord, cancellationToken);
        _transactionManager.RequireNew();
    }

    public async Task<Job> GetJobById(string jobId, CancellationToken cancellationToken = default)
    {
        var jobRecord = await _jobRepository.GetAsync(job => job.JobId == jobId, cancellationToken);
        if (jobRecord == null) 
        {
            return Job.None;
        }
        return jobRecord.ToJob(Enumerable.Empty<JobEvent>());
    }

    public async Task<Job> GetJobWithEventsById(string jobId, CancellationToken cancellationToken = default)
    {
        var job = await GetJobById(jobId, cancellationToken);
        if (job == Job.None)
        {
            return job;
        }
        job.Events = await _jobEventRepository.GetEventsByJobId(jobId, cancellationToken);
        return job;
    }

    public async Task<Job> GetPendingJobByUniqueId(string uniqueId, CancellationToken cancellationToken = default)
    {
        var record = await _jobRepository.GetAsync(job => job.UniqueId == uniqueId && job.Status != JobStatus.Success 
            && job.Status != JobStatus.Error, cancellationToken);
        if (record == null)
            return Job.None;
        return record.ToJob(Enumerable.Empty<JobEvent>());
   }
}
