using FluentValidation;
using MinimalApi.Endpoint.Extensions;
using ModuleDemo;
using ModuleDemo.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpoints()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(x => x.EnableAnnotations())
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddProblemDetails()
    .RegisterModules();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapEndpoints(root =>
{
    return root
        .AddEndpointFilter<FluentValidationFilter>()
        .MapGroup("api");
});
app.UseHttpsRedirection();
app.UseUncaughtExceptionHandler();

app.Run();

public partial class Program { }