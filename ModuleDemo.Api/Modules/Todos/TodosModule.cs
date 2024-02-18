using ModuleDemo.Modules.Todos.Endpoints;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos;

public class TodosModule : IModule
{
    public RouteGroupBuilder MapRouteGroup(RouteGroupBuilder global) =>
        global.MapGroup("todos");

    public void Register(IServiceCollection services, EndpointCollection endpoints)
    {
        services.AddScoped<ITodoService, TodoService>();

        endpoints
            .Add<CompleteTodo>()
            .Add<CreateTodo>()
            .Add<DeleteTodo>()
            .Add<GetAllTodos>()
            .Add<GetTodo>()
            .Add<UpdateTodo>();
    }
}
