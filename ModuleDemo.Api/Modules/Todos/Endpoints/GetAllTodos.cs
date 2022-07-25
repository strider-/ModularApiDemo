using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

public class GetAllTodos : IEndpoint<IResult>
{
    private ITodoService _service = null!;

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/todos", async (ITodoService service) =>
        {
            _service = service;
            return await HandleAsync();
        })
        .Produces<Todo[]>()
        .WithDisplayName(nameof(GetAllTodos))
        .WithOpenApi(
            summary: "Returns all todo items",
            description: "Gives you every task on your todo list, completed or not.",
            tags: nameof(Todo)
        );
    }

    public async Task<IResult> HandleAsync()
    {
        var result = await _service.GetAllTodos();

        return Results.Ok(result);
    }
}