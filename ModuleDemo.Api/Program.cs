using MinimalApi.Endpoint.Extensions;
using ModuleDemo;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpoints()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(x => x.EnableAnnotations())
    .RegisterModules();

var app = builder.Build();

app.UseSwaggerUI();
app.UseSwagger();
app.MapEndpoints();
app.UseHttpsRedirection();
app.UseImplicitValidation();

app.Run();

/// <summary>
/// Only used as the generic argument for WebApplicationFactory in tests.
/// </summary>
public class ModuleDemoApi { }