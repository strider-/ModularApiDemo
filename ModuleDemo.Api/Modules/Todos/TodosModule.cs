using FluentValidation;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos;

public class TodosModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IValidator<CreateTodoRequest>, CreateTodoRequest.Validator>();
        services.AddScoped<IValidator<UpdateTodoRequest>, UpdateTodoRequest.Validator>();
        
        return services;
    }
}
