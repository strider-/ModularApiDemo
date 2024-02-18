using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos;

public class TodosModule : IModule
{
    public RouteGroupBuilder MapRouteGroup(RouteGroupBuilder global) =>
        global.MapGroup("todos");

    public void Register(IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
    }
}
