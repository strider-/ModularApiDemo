using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

public class UpdateTodo : IEndpoint<IResult, int, UpdateTodoRequest>
{
    private ITodoService _service = null!;

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/todo/{id}", async (int id, UpdateTodoRequest request, ITodoService service) =>
        {
            _service = service;

            return await HandleAsync(id, request);
        })
        .Produces<Todo>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .WithDisplayName(nameof(UpdateTodo))
        .WithOpenApi(
            summary: "Updates a todo item",
            description: @"Change the title, description or set whether or not the task is complete. 
                           All fields are optional, but you must update at least 1 field.",
            tags: nameof(Todo)
        );
    }

    public async Task<IResult> HandleAsync(int id, UpdateTodoRequest request)
    {
        var todo = await _service.UpdateTodo(
            id, 
            request.Title, 
            request.Description, 
            request.Completed);

        if(todo == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(todo);
    }
}
