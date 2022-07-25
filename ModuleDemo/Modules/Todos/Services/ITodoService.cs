using ModuleDemo.Modules.Todos.Models;

namespace ModuleDemo.Modules.Todos.Services;

public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllTodos();

    Task<Todo?> GetTodo(int id);

    Task<Todo?> DeleteTodo(int id);

    Task<Todo> Create(string title, string description);

    Task<Todo?> UpdateTodo(int id, string? title = null, string? description = null, bool? completed = null);
}
