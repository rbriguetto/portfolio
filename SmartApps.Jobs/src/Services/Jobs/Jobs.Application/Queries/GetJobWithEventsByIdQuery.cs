using MediatR;
using SmartApps.Jobs.Application.ViewModels;

namespace SmartApps.Jobs.Application.Query;

public class GetJobWithEventsByIdQuery : IRequest<JobViewModel>
{
    public string JobId { get; set; } = string.Empty;
}