using MediatR;

namespace SmartApps.Jobs.Application.Commands;

public class MarkJobAsFinishedCommand : IRequest<bool>
{
    public string JobId { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = true;
    public string Response { get; set; } = string.Empty;
}