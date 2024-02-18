using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

[Module<TodosModule>]
public class CompleteTodo : IEndpoint<IResult, int, ITodoService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app
            .MapPut("{id}/done", HandleAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithDisplayName(nameof(CompleteTodo))
            .WithOpenApi(
                summary: "Marks a todo item complete",
                description: "Done with a task? Use this endpoint to mark it completed.",
                tags: nameof(Todo)
            );
    }

    public async Task<IResult> HandleAsync(int id, ITodoService service)
    {
        var todo = await service.UpdateTodo(id, completed: true);

        if(todo == null)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}
