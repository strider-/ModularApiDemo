using ModuleDemo.Modules.Todos.Endpoints;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos;

public class TodosModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        
        return services;
    }

    public RouteGroupBuilder MapRouteGroup(RouteGroupBuilder global) =>
        global.MapGroup("todos");

    public IEnumerable<Type> Endpoints => [
        typeof(CompleteTodo),
        typeof(CreateTodo),
        typeof(DeleteTodo),
        typeof(GetAllTodos),
        typeof(GetTodo),
        typeof(UpdateTodo)
    ];
}
