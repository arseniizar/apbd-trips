using TripApp.Application.Repository;
using TripApp.Infrastructure.Repository;

namespace TripApp.Infrastructure;

public static class InfrastructureServicesExtension
{
    public static void RegisterInfraServices(this IServiceCollection app)
    {
        app.AddScoped<ITripRepository, TripRepository>();
        app.AddScoped<IClientRepository, ClientRepository>();
        app.AddDbContext<TripContext>();
    }
}