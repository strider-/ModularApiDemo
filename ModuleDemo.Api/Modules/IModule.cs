namespace ModuleDemo.Modules;

public interface IModule
{
    void Register(IServiceCollection services);

    RouteGroupBuilder MapRouteGroup(RouteGroupBuilder global) => global;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ModuleAttribute<T> : Attribute where T : IModule
{
    public Type ModuleType => typeof(T);
}