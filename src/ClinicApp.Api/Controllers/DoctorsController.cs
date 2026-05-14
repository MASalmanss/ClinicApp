using ClinicApp.Api.Extensions;
using ClinicApp.Application.Doctors.Commands.CreateDoctor;
using ClinicApp.Application.Doctors.Queries.GetAllDoctors;
using ClinicApp.Application.Doctors.Queries.GetDoctorById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllDoctorsQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDoctorByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDoctorCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return result.ToActionResult();

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }
}
