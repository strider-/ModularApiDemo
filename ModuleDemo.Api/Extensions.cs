using FluentValidation;
using ModuleDemo.Modules;
using Swashbuckle.AspNetCore.Annotations;

namespace ModuleDemo;

public static class ModuleExtensions
{
    /// <summary>
    /// Every implmentation of <see cref="IModule"/> will be discovered and have its 
    /// <see cref="IModule.RegisterModule(IServiceCollection)"/> method invoked with the
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        var modules = DiscoverModules();

        foreach (var module in modules)
        {
            module.RegisterModule(services);
        }

        return services;
    }

    private static IEnumerable<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// When the incoming request model has an <see cref="IValidator{T}"/> registered in the DI container, 
    /// it will automatically be validated. A request with a valid model will move on to its handler, and 
    /// a request with an invalid model will generate a problem detail response to the client without invoking its handler.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseImplicitValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ImplicitValidation>();
    }
}
    

public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Registers tags and Swagger summary &amp; description information for the endpoint.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="summary"></param>
    /// <param name="description"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static RouteHandlerBuilder WithOpenApi(
        this RouteHandlerBuilder builder, 
        string? summary = null, 
        string? description = null, 
        params string[] tags)
    {
        return builder
            .WithTags(tags)
            .WithMetadata(new SwaggerOperationAttribute(summary, description));
    }
}

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP PATCH requests
    /// for the specified pattern.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="handler">The <see cref="Delegate" /> executed when the endpoint is matched.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder MapPatch(
            this IEndpointRouteBuilder endpoints,
            string pattern,
            Delegate handler)
    {
        return endpoints.MapMethods(pattern, new[] { HttpMethods.Patch }, handler);
    }
}