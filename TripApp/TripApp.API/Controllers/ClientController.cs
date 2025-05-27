using Microsoft.AspNetCore.Mvc;
using TripApp.Application.Exceptions;
using TripApp.Application.Services.Interfaces;

namespace Trip.API.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController(IClientService clientService) : ControllerBase
{
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteClient(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isRemoved = await clientService.DeleteClientAsync(id);
            return isRemoved ? NoContent() : NotFound();
        }
        catch (ClientExceptions.ClientHasTripsException e)
        {
            return BadRequest(e.Message);
        }
    }
}