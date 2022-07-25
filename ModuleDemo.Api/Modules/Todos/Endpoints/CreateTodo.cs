using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

public class CreateTodo : IEndpoint<IResult, CreateTodoRequest>
{
    private ITodoService _service = null!;
    private LinkGenerator _linker = null!;

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("api/todos", async (
            CreateTodoRequest request,
            LinkGenerator linker,
            ITodoService service) =>
        {
            _service = service;
            _linker = linker;

            return await HandleAsync(request);
        })
        .Produces<Todo>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .WithDisplayName(nameof(CreateTodo))
        .WithOpenApi(
            summary: "Creates a new todo item",
            description: "Keep track of your things to do by creating them here. Just need a title and description.",
            tags: nameof(Todo)
        );
    }

    public async Task<IResult> HandleAsync(CreateTodoRequest request)
    {
        var result = await _service.Create(request.Title, request.Description);
        var resultUrl = _linker.GetPathByName(GetTodo.Name, new { id = result.Id });

        return Results.Created(resultUrl!, result);
    }
}
