using Microsoft.AspNetCore.Mvc;
using TripApp.Application.DTOs;
using TripApp.Application.Exceptions;
using TripApp.Application.Services.Interfaces;
using TripApp.Core.Model;

namespace Trip.API.Controllers;

[ApiController]
[Route("api/trips")]
public class TripController(
    ITripService tripService)
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

    [HttpPost("{idTrip}/clients")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignClientToTrip(
        [FromRoute] int idTrip,
        [FromBody] AssignClientToTripDto assignClientDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await tripService.AssignClientToTripAsync(idTrip, assignClientDto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (TripExceptions.TripNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BaseExceptions.NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (TripExceptions.TripNameMismatchException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BaseExceptions.ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (TripExceptions.TripDateInPastException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ClientExceptions.ClientWithPeselExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ClientExceptions.ClientAlreadyRegisteredForTripException ex)
        {
            return Conflict(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred while processing your request.");
        }
    }
}