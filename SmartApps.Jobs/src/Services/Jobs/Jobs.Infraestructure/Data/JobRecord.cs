using Infraestructure.Data;
using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Infraestructure.Data;

public class JobRecord : IRecord
{
    public virtual int Id { get; set; } = 0;
    public virtual string JobId { get; set; } = string.Empty;
    public virtual string UniqueId { get; set; } = string.Empty;
    public virtual string Handler { get; set; } = string.Empty;
    public virtual string Payload { get; set; } = string.Empty;
    public virtual DateTime CreateUtc { get; set; } = DateTime.UtcNow;
    public virtual DateTime? StartedAtUtc { get; set; } = null;
    public virtual DateTime? FinishedAtUtc { get; set; } = null;
    public virtual string Response { get; set; } = string.Empty;
    public virtual string UserAgent { get; set; } = string.Empty;
    public virtual JobStatus Status { get; set; } = JobStatus.Pending;
}