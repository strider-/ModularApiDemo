using ModuleDemo.Modules.Todos.Models;
using System.Collections.Concurrent;

namespace ModuleDemo.Modules.Todos.Services;

public class TodoService : ITodoService
{
    private static ConcurrentBag<Todo> Store = new ConcurrentBag<Todo>();
    private static object _padLock = new object();

    public Task<Todo> Create(string title, string description)
    {
        var todo = new Todo
        {
            Id = NextId(),
            Title = title,
            Description = description,
            Completed = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        Store.Add(todo);

        return Task.FromResult(todo);
    }

    public Task<Todo?> DeleteTodo(int id)
    {
        lock (_padLock)
        {
            var todo = Store.SingleOrDefault(s => s.Id == id);

            if (todo == null)
            {
                return Task.FromResult((Todo?)null);
            }

            Store = new ConcurrentBag<Todo>(Store.Except(new[] { todo }));

            return Task.FromResult((Todo?)todo);
        }
    }

    public Task<IEnumerable<Todo>> GetAllTodos()
    {
        return Task.FromResult(Store.AsEnumerable());
    }

    public Task<Todo?> GetTodo(int id)
    {
        var todo = Store.SingleOrDefault(x => x.Id == id);

        return Task.FromResult(todo);
    }

    public Task<Todo?> UpdateTodo(int id, string? title = null, string? description = null, bool? completed = null)
    {
        var todo = Store.SingleOrDefault(x => x.Id == id);
        if (todo == null)
        {
            return Task.FromResult((Todo?)null);
        }

        todo.Title = title ?? todo.Title;
        todo.Description = description ?? todo.Description;
        todo.Completed = completed ?? todo.Completed;

        return Task.FromResult((Todo?)todo);
    }

    private int NextId()
    {
        lock (_padLock)
        {
            return Store.Any()
                ? Store.Max(x => x.Id) + 1
                : 1;
        }
    }
}
