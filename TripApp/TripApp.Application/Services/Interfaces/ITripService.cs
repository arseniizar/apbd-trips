﻿using TripApp.Application.DTOs;
using TripApp.Core.Model;

namespace TripApp.Application.Services.Interfaces;

public interface ITripService
{
    Task<PaginatedResult<GetTripDto>> GetPaginatedTripsAsync(int page = 1, int pageSize = 10);
    Task<IEnumerable<GetTripDto>> GetAllTripsAsync();

    Task AssignClientToTripAsync(int tripId, AssignClientToTripDto clientData,
        CancellationToken cancellationToken = default);
}