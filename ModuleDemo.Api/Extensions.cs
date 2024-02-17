using FluentValidation;
using ModuleDemo.Api;
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

    /// <summary>
    /// Validates the request body for this endpoint. Request body model must have an IValidator implementation
    /// defined for validation to occur. Includes a call to ProducesValidationProblem for OpenApi metadata.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static RouteHandlerBuilder ValidateRequestBody(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<FluentValidationFilter>()
            .ProducesValidationProblem();
    }
}