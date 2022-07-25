using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

public class CompleteTodo : IEndpoint<IResult, int>
{
    private ITodoService _service = null!;

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPut("api/todo/{id}/done", async (int id, ITodoService service) =>
        {
            _service = service;
            return await HandleAsync(id);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithDisplayName(nameof(CompleteTodo))
        .WithOpenApi(
            summary: "Marks a todo item complete",
            description: "Done with a task? Use this endpoint to mark it completed.",
            tags: nameof(Todo)
        );
    }

    public async Task<IResult> HandleAsync(int id)
    {
        var todo = await _service.UpdateTodo(id, completed: true);
        if(todo == null)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}
