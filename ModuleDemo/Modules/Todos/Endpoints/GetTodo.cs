using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

public class GetTodo : IEndpoint<IResult, int>
{
    public const string Name = nameof(GetTodo);

    private ITodoService _service = null!;

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("api/todo/{id}", async (int id, ITodoService service) =>
        {
            _service = service;
            return await HandleAsync(id);
        })
        .Produces<Todo>()
        .Produces(StatusCodes.Status404NotFound)
        .WithName(Name)
        .WithDisplayName(Name)
        .WithOpenApi(
            summary: "Returns a specific todo item",
            description: "Get detail on a specific task by giving the ID of the task.",
            tags: nameof(Todo)
        );
    }

    public async Task<IResult> HandleAsync(int id)
    {
        var result = await _service.GetTodo(id);
        if(result == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result);
    }
}