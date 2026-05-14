using ClinicApp.Api.Extensions;
using ClinicApp.Application.Appointments.Commands.BookAppointment;
using ClinicApp.Application.Appointments.Commands.CancelAppointment;
using ClinicApp.Application.Appointments.Commands.ConfirmAppointment;
using ClinicApp.Application.Appointments.Queries.GetAllAppointments;
using ClinicApp.Application.Appointments.Queries.GetAppointmentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllAppointmentsQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Book(BookAppointmentCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return result.ToActionResult();

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ConfirmAppointmentCommand(id), ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelAppointmentCommand(id), ct);
        return result.ToActionResult();
    }
}
