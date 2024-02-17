namespace ModuleDemo.Modules;

public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection services);

    RouteGroupBuilder MapRouteGroup(RouteGroupBuilder global) => global;

    IEnumerable<Type> Endpoints { get; }
}