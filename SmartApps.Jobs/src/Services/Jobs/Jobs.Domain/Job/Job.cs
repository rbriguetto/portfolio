using System.Security.Cryptography;
using System.Text;

namespace SmartApps.Jobs.Domain;

public class Job
{
    public static readonly Job None = new Job("None", "None");

    public string JobId { get; private set; } = string.Empty;
    public string Handler { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; } = null;
    public DateTime? FinishedAtUtc { get; set; } = null;
    public JobStatus Status { get; private set; } = JobStatus.Pending;
    public string UniqueId { get; private set; } = string.Empty;
    public IEnumerable<JobEvent> Events { get; set; } = Enumerable.Empty<JobEvent>();

    public Job(string handler, string payload)
    {
        if (string.IsNullOrEmpty(handler))
            throw new ArgumentException("Invalid argument", nameof(handler));
        if (string .IsNullOrEmpty(payload))
            throw new ArgumentException("Invalid argument", nameof(payload));

        JobId = Guid.NewGuid().ToString();
        Handler = handler;
        Payload = payload;
        Status = JobStatus.Pending;
        UniqueId = CreateUniqueId();
    }

    public Job(string jobId, string handler, string payload, JobStatus status, DateTime createUtc, 
        DateTime? startedAtUtc, DateTime? finishedAtUtc, string userAgent, string response,
        IEnumerable<JobEvent> events)
    {
        if (string.IsNullOrEmpty(handler))
            throw new ArgumentException("Invalid argument", nameof(handler));
        if (string .IsNullOrEmpty(payload))
            throw new ArgumentException("Invalid argument", nameof(payload));

        JobId = jobId;
        Handler = handler;
        Payload = payload;
        Status = status;
        CreatedUtc = createUtc;
        StartedAtUtc = startedAtUtc;
        FinishedAtUtc = finishedAtUtc;
        UserAgent = userAgent;
        Response = response;
        Events = events;
        UniqueId = CreateUniqueId();
    }

    public Job MarkAsStarted()
    {
        if (Status != JobStatus.Pending)
            throw new JobAlreadyStartedException();
        Status = JobStatus.Started;
        StartedAtUtc = DateTime.UtcNow;
        return this;
    }

    public Job MarkAsFinished(bool withSuccess = true)
    {
        if (Status == JobStatus.Success || Status == JobStatus.Error)
            throw new JobAlreadyFinishedException();
        Status = withSuccess ? JobStatus.Success : JobStatus.Error;
        FinishedAtUtc = DateTime.UtcNow;
        return this;
    }

    public Job WithResponse(string response)
    {
        Response = response;
        return this;
    }

    public string CreateUniqueId()
    {
        var input = $"{Handler}_{Payload}";
        byte[] textBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(textBytes);
        string hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", string.Empty);
        return hash;
    }
}
