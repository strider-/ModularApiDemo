namespace ModuleDemo.Modules;

public interface IModule
{
    void Register(IServiceCollection services, EndpointCollection endpoints);

    RouteGroupBuilder MapRouteGroup(RouteGroupBuilder global) => global;
}