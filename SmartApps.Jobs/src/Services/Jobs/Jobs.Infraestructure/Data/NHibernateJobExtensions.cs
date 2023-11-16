using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Infraestructure.Data;

public static class NHibernateJobExtensions
{
    public static Job ToJob(this JobRecord record, IEnumerable<JobEvent> events) => 
        new Job(record.JobId, record.Handler, record.Payload, record.Status, 
            record.CreateUtc, record.StartedAtUtc, record.FinishedAtUtc, record.UserAgent, 
            record.Response, events);  
}