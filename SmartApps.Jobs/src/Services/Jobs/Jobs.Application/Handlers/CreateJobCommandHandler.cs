using MediatR;
using Microsoft.Extensions.Logging;
using SmartApps.Jobs.Application.Commands;
using SmartApps.Jobs.Application.Notifications;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.Application.Handlers;

public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, CreateJobResponse>
{
    private readonly IJobRepository _jobRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateJobCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJobCommandHandler(
        IJobRepository jobRepository,
        IMediator mediator,
        IUnitOfWork unitOfWork,
        ILogger<CreateJobCommandHandler> logger)
    {
        _jobRepository = jobRepository; 
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateJobResponse> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var job = new Job(request.Handler, request.Payload);
            var pendingJob = await _jobRepository.GetPendingJobByUniqueId(job.UniqueId, cancellationToken);
            if (pendingJob != Job.None)
            {
                return new CreateJobResponse() { JobId = pendingJob.JobId };
            }
            _unitOfWork.BeginTransaction();
            await _jobRepository.CreateJob(job, cancellationToken);
            _unitOfWork.Commit();
            _logger.LogInformation("Tarefa criada com id {job.id} e processador {job.handler}. Aguardando processamento ...", job.JobId, job.Handler);
            await _mediator.Publish(JobCreatedNotification.Create(job), cancellationToken);
            return new CreateJobResponse() { JobId = job.JobId };
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Failed to create job {request.Handler}. {error_msg} {stack_trace}", 
                request.Handler, ex.Message, ex.StackTrace);
            throw;
        }
    }

    
}