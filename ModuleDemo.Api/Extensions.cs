using FluentValidation;
using MinimalApi.Endpoint;
using ModuleDemo.Modules;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

namespace ModuleDemo;

public static class ModuleExtensions
{
    /// <summary>
    /// Every implmentation of <see cref="IModule"/> will be discovered and have its 
    /// <see cref="IModule.Register(IServiceCollection)"/> method invoked with the
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        foreach (var module in ModuleDiscovery.GetModules())
        {
            module.Register(services);
        }

        return services;
    }
}

public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Registers all <see cref="IEndpoint"/> implentations, with optional configuration of a <see cref="RouteGroupBuilder"/> that 
    /// will apply to all endpoints
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="globalGroupConfig"></param>
    public static void MapEndpoints(this WebApplication builder, Func<RouteGroupBuilder, RouteGroupBuilder>? globalGroupConfig = null)
    {
        var globalGroup = builder.MapGroup(string.Empty);
        globalGroup = globalGroupConfig?.Invoke(globalGroup) ?? globalGroup;

        using var scope = builder.Services.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            var module = ModuleDiscovery.GetModuleByEndpoint(endpoint);

            var moduleGroup = module.MapRouteGroup(globalGroup);

            endpoint.AddRoute(moduleGroup);
        }

        ModuleDiscovery.Complete();
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

file static class ModuleDiscovery
{
    private static Lazy<IEnumerable<ModuleInfo>> Modules = new(DiscoverModules);

    public static IEnumerable<IModule> GetModules() => Modules.Value.Select(m => m.Module);

    public static void Complete() => Modules = new();

    public static IModule GetModuleByEndpoint(IEndpoint endpoint)
    {
        var moduleType = endpoint.GetType()
            .GetCustomAttribute(typeof(ModuleAttribute<>))!
            .GetType()
            .GetGenericArguments()
            .Single();

        return Modules.Value.Single(mi => mi.Type == moduleType).Module;
    }

    private static IEnumerable<ModuleInfo> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(t => t.IsClass && t.IsAssignableTo(typeof(IModule)))
            .Select(t => new ModuleInfo((IModule)Activator.CreateInstance(t)!, t));
    }

    record ModuleInfo(IModule Module, Type Type);
}