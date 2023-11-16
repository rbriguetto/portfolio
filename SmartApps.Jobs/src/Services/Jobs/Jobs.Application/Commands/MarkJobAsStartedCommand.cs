using MediatR;

namespace SmartApps.Jobs.Application.Commands;

public class MarkJobAsStartedCommand : IRequest<bool>
{
    public string JobId { get; set; } = string.Empty;
}