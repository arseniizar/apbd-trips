using Microsoft.Extensions.DependencyInjection;
using Trip.Infrastructure.Repository;
using TripApp.Application.Repository;

namespace Trip.Infrastructure;

public static class InfrastructureServicesExtension
{
    public static void RegisterInfraServices(this IServiceCollection app)
    {
        app.AddScoped<ITripRepository, TripRepository>();
        app.AddDbContext<TripdbContext>();
    }
}