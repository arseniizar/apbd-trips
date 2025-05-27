using Microsoft.AspNetCore.Mvc;
using TripApp.Application.Exceptions;
using TripApp.Application.Services.Interfaces;

namespace TripApp.Presentation.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController(IClientService clientService) : ControllerBase
{
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteClient(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isRemoved = await clientService.DeleteClientAsync(id);
            return isRemoved ? Ok() : NotFound();
        }
        catch (ClientExceptions.ClientHasTripsException e)
        {
            return BadRequest(e.Message);
        }
    }
}