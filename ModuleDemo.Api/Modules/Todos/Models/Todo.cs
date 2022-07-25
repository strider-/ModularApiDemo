namespace ModuleDemo.Modules.Todos.Models;

public class Todo
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Completed { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
