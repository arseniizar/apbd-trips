using TripApp.Application.Services;
using TripApp.Application.Services.Interfaces;

namespace TripApp.Application;

public static class ApplicationServicesExtension
{
    public static void RegisterApplicationServices(this IServiceCollection app)
    {
        app.AddScoped<ITripService, TripService>();
        app.AddScoped<IClientService, ClientService>();
    }
}