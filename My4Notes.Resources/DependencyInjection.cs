using Microsoft.Extensions.DependencyInjection;

namespace My4Notes.Resources;

/// <summary>
/// Contains extension methods for setting up dependencies related to the application.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds and configures services related to the application.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The IServiceCollection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;

    }
}