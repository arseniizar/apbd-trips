using TripApp.Application.DTOs;
using TripApp.Core.Model;

namespace TripApp.Application.Mappers;

public static class TripMapper
{
    public static GetTripDto MapToGetTripDto(this Trip trip)
    {
        return new GetTripDto
        {
            Name = trip.Name,
            DateFrom = trip.DateFrom,
            DateTo = trip.DateTo,
            Description = trip.Description,
            MaxPeople = trip.MaxPeople,
            Countries = trip.IdCountries.Select(country => country.MapToCountryDto()).ToList(),
            Clients = trip.ClientTrips.Select(e => e.IdClientNavigation.MapToCountryDto()).ToList()
        };
    }
}