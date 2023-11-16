using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApps.Jobs.Application.Commands;
using SmartApps.Jobs.Application.Query;
using SmartApps.Jobs.Application.ViewModels;
using SmartApps.Jobs.Domain;

namespace SmartApps.Jobs.Api.Controllers;

[ApiController]
[AllowAnonymous()]
[Route("/api/[controller]/[action]")]
public class JobController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateJobResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateJob([FromBody] CreateJobCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return new ObjectResult(result) { StatusCode = 201 };
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [HttpGet()]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetJobById([FromQuery] string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetJobByIdQuery() { JobId = id }, cancellationToken);
            return Ok(result);
        }
        catch (JobNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet()]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetJobWithEventsById([FromQuery] string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetJobWithEventsByIdQuery() { JobId = id }, cancellationToken);
            return Ok(result);
        }
        catch (JobNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet()]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<JobEvent>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetJobEvents([FromQuery] string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetJobEventsQuery() { JobId = id }, cancellationToken);
            return Ok(result);
        }
        catch (JobNotFoundException)
        {
            return NotFound();
        }
    }
}