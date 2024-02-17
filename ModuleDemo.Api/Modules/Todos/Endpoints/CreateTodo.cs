using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

public class CreateTodo : IEndpoint<IResult, CreateTodoRequest, LinkGenerator, ITodoService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app
            .MapPost("api/todos", HandleAsync)
            .ValidateRequestBody()
            .Produces<Todo>(StatusCodes.Status201Created)
            .WithDisplayName(nameof(CreateTodo))
            .WithOpenApi(
                summary: "Creates a new todo item",
                description: "Keep track of your things to do by creating them here. Just need a title and description.",
                tags: nameof(Todo)
            );
    }

    public async Task<IResult> HandleAsync(CreateTodoRequest request, LinkGenerator linker, ITodoService service)
    {
        var result = await service.Create(request.Title, request.Description);
        var resultUrl = linker.GetPathByName(GetTodo.Name, new { id = result.Id });

        return Results.Created(resultUrl!, result);
    }
}
