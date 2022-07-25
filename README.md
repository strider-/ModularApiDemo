# .NET 6 Module-Based Minimal API Demo

An example of a API taking a module-based architecture approach, using endpoint based routing in .NET 6. Inspired by these [blog](https://timdeschryver.dev/blog/maybe-its-time-to-rethink-our-project-structure-with-dot-net-6) [posts](https://timdeschryver.dev/blog/the-simplicity-of-net-endpoints) 
from [Tim Deschryver](https://timdeschryver.dev/).

----------

## Technology Stack
### API Project
* .NET 6 / C# 10
* [FluentValidation](https://docs.fluentvalidation.net/) v11.1.0
* [MinimalApi.Endpoint](https://github.com/michelcedric/StructuredMinimalApi) v1.2.0
* [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) w/ [Annotations](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#additional-packages) v6.4.0
### Tests Project
* [xUnit](https://xunit.net/) v2.4.1
* [FluentAssertions](https://fluentassertions.com/) v6.7.0
* Microsoft.AspNetCore.Mvc.Testing 6.0.7

## Project Structure
```
API
 ├── Modules/               // All modules should be under here
 │   └── Todos/             // The 'Todo' module root
 │       ├── Endpoints/     // Each endpoint for the module is its own class here
 │       ├── Models/        // Domain model and DTOs
 │       ├── Services/      // Interfaces w/ implementations
 │       └── TodoModule.cs  // Registers all services w/ DI
 │   IModule.cs             // Modules implement need to implement this interface
 │ Extensions.cs            // DI registration for modules, validation middlware
 │ Program.cs               
 │ ImplicitValidation.cs    // Validates incoming request models automatically
```
While domain models and service implementations are in this project for the demo, in a more rubust application they would probably exist elsewhere.

## Getting Started
Just clone the project and run it, no configuration / external dependencies needed. 