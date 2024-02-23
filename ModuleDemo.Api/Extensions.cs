using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using MinimalApi.Endpoint;
using ModuleDemo.Modules;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Immutable;
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
        foreach (var module in MetadataDiscovery.GetModules())
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

        var map = new Dictionary<IModule, RouteGroupBuilder>();
        foreach (var endpoint in endpoints)
        {
            var module = MetadataDiscovery.GetEndpointModule(endpoint);

            if (!map.TryGetValue(module, out var moduleGroup))
            {
                moduleGroup = module.MapRouteGroup(globalGroup);
                map[module] = moduleGroup;
            }

            endpoint.AddRoute(moduleGroup);
        }

        MetadataDiscovery.Complete();
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

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseUncaughtExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseExceptionHandler(app =>
            app.Run(async (ctx) =>
            {
                var exceptionDetails = ctx.Features.Get<IExceptionHandlerFeature>();
                var statusCode = 500;
                var title = "An unexpected error occurred while processing your request";

                if (exceptionDetails?.Error is BadHttpRequestException br)
                {
                    statusCode = br.StatusCode;
                    title = br.InnerException?.Message ?? br.Message;
                }

                await TypedResults.Problem(statusCode: statusCode, title: title).ExecuteAsync(ctx);
            })
        );
    } 
}

file static class MetadataDiscovery
{
    private static Lazy<IEnumerable<Metadata>> Modules = new(DiscoverModulesWithEndpoints);

    public static IEnumerable<IModule> GetModules() => Modules.Value.Select(m => m.Module);

    public static void Complete() => Modules = new();

    public static IModule GetEndpointModule(IEndpoint endpoint) =>
        Modules.Value.Single(m => m.EndpointTypes.Contains(endpoint.GetType())).Module;

    private static IEnumerable<Metadata> DiscoverModulesWithEndpoints()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpoint)))
            .Select(ep => new
            {
                EndpointType = ep,
                ModuleType = ep.GetCustomAttribute(typeof(ModuleAttribute<>))?
                            .GetType()
                            .GetGenericArguments()
                            .Single()
                            ??
                            throw new Exception($"{ep.FullName} is missing a Module attribute declaration")
            })
            .GroupBy(x => x.ModuleType)
            .Select(g => new Metadata
            (
                (IModule)Activator.CreateInstance(g.Key)!,
                g.Select(x => x.EndpointType).ToImmutableArray()
            ))
            .ToImmutableArray();
    }

    record Metadata(IModule Module, IEnumerable<Type> EndpointTypes);
}