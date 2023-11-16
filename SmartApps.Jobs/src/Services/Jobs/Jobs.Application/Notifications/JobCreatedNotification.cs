using MediatR;
using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Application.Notifications;

public class JobCreatedNotification : INotification
{
    public string JobId { get; set; } = string.Empty;
    public string Handler { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;

    public static JobCreatedNotification Create(Job job) => new JobCreatedNotification() {
        JobId = job.JobId,
        Handler = job.Handler,
        Payload = job.Payload
    };
}