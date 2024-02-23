using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Options;

namespace ModuleDemo.Api;

public class FluentValidationFilter : IEndpointFilter
{
    private static readonly Type ValidatorType = typeof(IValidator<>);
    private static readonly Type ValidationContextType = typeof(ValidationContext<>);

    private readonly IOptions<JsonOptions> _jsonOptions;

    public FluentValidationFilter(IOptions<JsonOptions> jsonOptions)
    {
        _jsonOptions = jsonOptions;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var requestType = httpContext.GetEndpoint()?
            .Metadata
            .GetMetadata<IAcceptsMetadata>()?
            .RequestType;

        if (requestType == null)
        {
            // endpoint is not accepting a model
            return await next(context);
        }

        var validator = GetValidator(httpContext, requestType);
        if (validator == null)
        {
            // no registered IValidator<T> for the model
            return await next(context);
        }

        var model = context.Arguments.SingleOrDefault(arg => arg?.GetType() == requestType);
        var result = await ValidateRequestModelAsync(validator, requestType, model);

        if (!result.IsValid)
        {
            // invalid model, return problem details to client
            return ValidationProblem(result);
        }

        // valid model, keep the request pipeline going.
        return await next(context);
    }

    private IValidator? GetValidator(HttpContext context, Type modelType)
    {
        var validatorType = ValidatorType.MakeGenericType(modelType);
        return context.RequestServices.GetService(validatorType) as IValidator;
    }

    private async Task<ValidationResult> ValidateRequestModelAsync(IValidator validator, Type modelType, object? model)
    {
        var vcType = ValidationContextType.MakeGenericType(modelType);
        var vcInst = Activator.CreateInstance(vcType, [model]) as IValidationContext;

        return await validator.ValidateAsync(vcInst);
    }

    private IResult ValidationProblem(ValidationResult result)
    {
        // Configure<JsonOptions> seems to have no effect when serializing
        // dictionary keys for problem details, so let's explicitly enforce policies here.
        // We'll check for a DictionaryKeyPolicy, then a PropertyNamingPolicy, falling
        // back to the property name as-is if neither are set.

        var namingPolicy =
            _jsonOptions.Value.SerializerOptions.DictionaryKeyPolicy ??
            _jsonOptions.Value.SerializerOptions.PropertyNamingPolicy;

        var errors = result.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => namingPolicy?.ConvertName(g.Key) ?? g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

        return TypedResults.ValidationProblem(errors);
    }
}
