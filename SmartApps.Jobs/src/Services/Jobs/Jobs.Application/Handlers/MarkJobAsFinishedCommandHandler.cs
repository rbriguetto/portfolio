using Microsoft.Extensions.Logging;
using MediatR;
using SmartApps.Jobs.Application.Commands;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.Application.Handlers;

public class MarkJobAsFinishedCommandHandler : IRequestHandler<MarkJobAsFinishedCommand, bool>
{
    private  readonly IJobRepository _jobRepository;
    private readonly ILogger<CreateJobCommandHandler> _logger;

    public MarkJobAsFinishedCommandHandler(
        IJobRepository jobRepository,
        ILogger<CreateJobCommandHandler> logger)
    {
        _jobRepository = jobRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(MarkJobAsFinishedCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetJobById(request.JobId, cancellationToken);
        if (job == Job.None)
        {
            throw new JobNotFoundException(request.JobId);
        }
        job.MarkAsFinished(request.IsSuccess).WithResponse(request.Response);
        await _jobRepository.UpdateJob(job, cancellationToken);
        _logger.LogInformation("Job {job.id} status changed to {job.status}", job.JobId, job.Status);
        return true;
    }
}