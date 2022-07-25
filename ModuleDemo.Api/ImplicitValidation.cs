using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Options;

namespace ModuleDemo;

public class ImplicitValidation
{
    private static Type ValidatorType = typeof(IValidator<>);
    private static Type ValidationContextType = typeof(ValidationContext<>);

    private readonly RequestDelegate _next;

    public ImplicitValidation(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IOptions<JsonOptions> jsonOptions)
    {
        var requestType = context.GetEndpoint()?
            .Metadata
            .GetMetadata<IAcceptsMetadata>()?
            .RequestType;

        if (requestType == null)
        {
            // endpoint is not accepting a model
            await _next.Invoke(context);
            return;
        }

        var validator = GetValidator(context, requestType);
        if (validator == null)
        {
            // no registered IValidator<T> for the model
            await _next.Invoke(context);
            return;
        }
        
        var model = await ReadModelBufferedAsync(context, requestType);
        var result = await ValidateRequestModelAsync(validator, requestType, model);

        if (!result.IsValid)
        {
            // invalid model, return problem details to client
            await ValidationProblem(result, jsonOptions).ExecuteAsync(context);
            return;
        }

        // valid model, keep the request pipeline going.
        await _next.Invoke(context);
    }

    private IValidator? GetValidator(HttpContext context, Type modelType)
    {
        var validatorType = ValidatorType.MakeGenericType(modelType);
        return context.RequestServices.GetService(validatorType) as IValidator;
    }

    private async ValueTask<object?> ReadModelBufferedAsync(HttpContext context, Type modelType)
    {
        context.Request.EnableBuffering();
        var obj = await context.Request.ReadFromJsonAsync(modelType);
        context.Request.Body.Seek(0, SeekOrigin.Begin);

        return obj;
    }

    private async Task<ValidationResult> ValidateRequestModelAsync(IValidator validator, Type modelType, object? model)
    {
        var vcType = ValidationContextType.MakeGenericType(modelType);
        var vcInst = Activator.CreateInstance(vcType, new[] { model }) as IValidationContext;

        return await validator.ValidateAsync(vcInst);
    }

    private IResult ValidationProblem(ValidationResult result, IOptions<JsonOptions> options)
    {
        // Configure<JsonOptions> seems to have no effect when serializing
        // dictionary keys for problem details, so let's explicitly enforce policies here.
        // We'll check for a DictionaryKeyPolicy, then a PropertyNamingPolicy, falling
        // back to the property name as-is if neither are set.

        var namingPolicy =
            options.Value.SerializerOptions.DictionaryKeyPolicy ??
            options.Value.SerializerOptions.PropertyNamingPolicy;

        var errors = result.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => namingPolicy?.ConvertName(g.Key) ?? g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

        return Results.ValidationProblem(errors);
    }
}