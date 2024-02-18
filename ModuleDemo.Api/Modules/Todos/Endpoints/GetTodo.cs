using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

[Module<TodosModule>]
public class GetTodo : IEndpoint<IResult, int, ITodoService>
{
    public const string Name = nameof(GetTodo);

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app
            .MapGet("{id}", HandleAsync)
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

    public async Task<IResult> HandleAsync(int id, ITodoService service)
    {
        var result = await service.GetTodo(id);
        if(result == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result);
    }
}