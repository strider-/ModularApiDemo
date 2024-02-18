# .NET 8 Module-Based Minimal API Demo

An example of a API taking a module-based architecture approach, using the [minimal APIs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0) / endpoint based routing in .NET 8. Inspired by these [blog](https://timdeschryver.dev/blog/maybe-its-time-to-rethink-our-project-structure-with-dot-net-6) [posts](https://timdeschryver.dev/blog/the-simplicity-of-net-endpoints) 
from [Tim Deschryver](https://timdeschryver.dev/).

----------

## Technology Stack
### API Project
* .NET 8 / C# 12
* [FluentValidation](https://docs.fluentvalidation.net/) w/ DependencyInjectionExtensions v11.9.0
* [MinimalApi.Endpoint](https://github.com/michelcedric/StructuredMinimalApi) v1.3.0
* [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) w/ [Annotations](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#additional-packages) v6.5.0
### Tests Project
* [xUnit](https://xunit.net/) v2.7.0
* [FluentAssertions](https://fluentassertions.com/) v6.12.0
* Microsoft.AspNetCore.Mvc.Testing 8.0.2

## Project Structure
```
API
 ├── Modules/               // All modules should be under here
 │   ├── Todos/             // The 'Todo' module root
 │   │   ├── Endpoints/     // Each endpoint for the module is its own class here
 │   │   ├── Models/        // Domain model and DTOs
 │   │   ├── Services/      // Interfaces w/ implementations
 │   │   └── TodoModule.cs  // Registers all module services, endpoints and routing
 │   └── IModule.cs         // Modules need to implement this interface
 ├── Extensions.cs          // DI registration for modules, router builder extensions
 ├── Program.cs             
 └── FluentValidationFilter.cs  // Endpoint filter for validating request bodies
```
While domain models and service implementations are in this project for the demo, in a more rubust application they would probably exist elsewhere.

## Getting Started
Just clone the project and run it, no configuration / external dependencies needed. 