using MediatR;

namespace SmartApps.Jobs.Application.Commands;

public class CreateJobCommand : IRequest<CreateJobResponse>
{
    public string Handler { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

public class CreateJobResponse
{
    public string JobId { get; set; } = string.Empty;
}