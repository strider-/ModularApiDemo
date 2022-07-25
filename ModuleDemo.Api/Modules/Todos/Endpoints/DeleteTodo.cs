using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

public class DeleteTodo : IEndpoint<IResult, int>
{
    private ITodoService _service = null!;

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/todo/{id}", async (int id, ITodoService service) =>
        {
            _service = service;
            return await HandleAsync(id);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .WithDisplayName(nameof(DeleteTodo))
        .WithOpenApi(
            summary: "Deletes a specific todo item",
            description: "Once you're completely done with a task (or maybe you made a mistake) delete it here.",
            tags: nameof(Todo)
        );
    }

    public async Task<IResult> HandleAsync(int id)
    {
        var result = await _service.DeleteTodo(id);
        if(result == null)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}
