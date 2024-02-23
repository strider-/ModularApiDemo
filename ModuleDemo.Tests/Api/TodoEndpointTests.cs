using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace ModuleDemo.Tests.Api;

[Trait("Api", "Todo")]
public class TodoEndpointTests(WebApplicationFactory<Program> app) 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = app.CreateClient();

    [Fact]
    public async Task GetAllTodos_Is_Successful()
    {
        var response = await _client.GetAsync("/api/todos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("title", "")]
    [InlineData("", "description")]
    [InlineData("", "")]
    public async Task CreateTodo_Validates_Request(string? title, string? description)
    {
        var content = new { title, description }.ToStringContent();

        var response = await _client.PostAsync("/api/todos", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTodo_Is_Successful()
    {
        var content = new { title = "title", description = "description" }.ToStringContent();

        var response = await _client.PostAsync("/api/todos", content);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.OriginalString.Should().MatchRegex("^/api/todos/\\d+$");
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetTodo_Returns_Not_Found()
    {
        var response = await _client.GetAsync("/api/todos/1337");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTodo_Is_Successful()
    {
        var location = await CreateTodoAsync();

        var response = await _client.GetAsync(location);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CompleteTodo_Returns_Not_Found()
    {
        var response = await _client.PutAsync("/api/todos/31337/done", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CompleteTodo_Is_Successful()
    {
        var location = await CreateTodoAsync();

        var response = await _client.PutAsync($"{location}/done", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTodo_Returns_Not_Found()
    {
        var response = await _client.DeleteAsync("/api/todos/42069");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_Is_Successful()
    {
        var location = await CreateTodoAsync();

        var response = await _client.DeleteAsync(location);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("", "", null)]
    [InlineData(null, "", null)]
    [InlineData("", null, null)]
    public async Task UpdateTodo_Validates_Request(string? title, string? description, bool? completed)
    {
        var location = await CreateTodoAsync();
        var content = new { title, description, completed }.ToStringContent();

        var response = await _client.PatchAsync(location, content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("title", null, null)]
    [InlineData(null, "desc", null)]
    [InlineData(null, null, true)]
    [InlineData("title", "desc", null)]
    [InlineData("title", null, true)]
    [InlineData(null, "desc", true)]
    [InlineData("title", "desc", true)]
    public async Task UpdateTodo_Is_Successful_With_At_Least_One_Field_Update(string? title, string? description, bool? completed)
    {
        var location = await CreateTodoAsync();
        var content = new { title, description, completed }.ToStringContent();

        var response = await _client.PatchAsync(location, content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<string> CreateTodoAsync(string title = "title", string description = "desc")
    {
        var r = await _client.PostAsync("/api/todos", new { title, description }.ToStringContent());

        return r.Headers.Location!.OriginalString;
    }
}
