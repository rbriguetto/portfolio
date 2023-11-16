using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Application.ViewModels;

public class JobViewModel
{
    public string JobId { get; set; } = string.Empty;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string Response { get; set; } = string.Empty;
    public IEnumerable<JobEvent> Events { get; set; } = Enumerable.Empty<JobEvent>();

    public static JobViewModel FromJob(Job job) => new JobViewModel()
    {
        JobId = job.JobId,
        Status = job.Status,
        Response = job.Response,
        Events = job.Events
    };
}