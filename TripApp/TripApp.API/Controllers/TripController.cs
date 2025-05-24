using Microsoft.AspNetCore.Mvc;
using TripApp.Application.DTOs;
using TripApp.Application.Exceptions;
using TripApp.Application.Services.Interfaces;
using TripApp.Core.Model;

namespace Trip.API.Controllers;

[ApiController]
[Route("api/trips")]
public class TripController(
    ITripService tripService,
    IClientService clientService) 
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<GetTripDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<GetTripDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrips(
        [FromQuery(Name = "page")] int? page,
        [FromQuery(Name = "pageSize")] int? pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page is null && pageSize is null)
        {
            var trips = await tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        var paginatedTrips = await tripService.GetPaginatedTripsAsync(page ?? 1, pageSize ?? 10);
        return Ok(paginatedTrips);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTrip(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isRemoved = await clientService.DeleteClientAsync(id);
            return isRemoved ? Ok() : StatusCode(500);
        }
        catch (BaseExceptions.NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ClientExceptions.ClientHasTripsException e)
        {
            return BadRequest(e.Message);
        }
    }
}