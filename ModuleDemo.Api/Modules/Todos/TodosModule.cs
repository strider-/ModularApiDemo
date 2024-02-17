using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos;

public class TodosModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        
        return services;
    }
}
