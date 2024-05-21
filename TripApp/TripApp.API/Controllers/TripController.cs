using Microsoft.AspNetCore.Mvc;
using TripApp.Application.Services.Interfaces;

namespace Trip.API.Controllers;

[ApiController]
[Route("api/trips")]
public class TripController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripController(ITripService tripService)
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