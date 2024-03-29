﻿using MinimalApi.Endpoint;
using ModuleDemo.Modules.Todos.Models;
using ModuleDemo.Modules.Todos.Services;

namespace ModuleDemo.Modules.Todos.Endpoints;

[Module<TodosModule>]
public class GetAllTodos : IEndpoint<IResult, ITodoService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app
            .MapGet(string.Empty, HandleAsync)
            .Produces<Todo[]>()
            .WithDisplayName(nameof(GetAllTodos))
            .WithOpenApi(
                summary: "Returns all todo items",
                description: "Gives you every task on your todo list, completed or not.",
                tags: nameof(Todo)
            );
    }

    public async Task<IResult> HandleAsync(ITodoService service)
    {
        var result = await service.GetAllTodos();

        return TypedResults.Ok(result);
    }
}