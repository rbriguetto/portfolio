using MediatR;
using SmartApps.Jobs.Application.Query;
using SmartApps.Jobs.Application.ViewModels;
using SmartApps.Jobs.Domain;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.Application.Handlers;

public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobViewModel>
{
    private readonly IJobRepository _jobRepository;

    public GetJobByIdQueryHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository; 
    }

    public async Task<JobViewModel> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetJobById(request.JobId, cancellationToken);
        if (job == Job.None)
        {
            throw new JobNotFoundException(request.JobId);
        }
        return JobViewModel.FromJob(job);
    }
}