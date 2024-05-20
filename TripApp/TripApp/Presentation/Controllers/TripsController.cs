using Microsoft.AspNetCore.Mvc;
using TripApp.Application.Services.Interfaces;

namespace TripApp.Presentation.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int? page, int? pageSize)
    {
        if (page is null || pageSize is null)
            return Ok(await _tripService.GetAllTripsAsync());

        return Ok(await _tripService.GetPaginatedTripsAsync(page.Value, pageSize.Value));
    }
}