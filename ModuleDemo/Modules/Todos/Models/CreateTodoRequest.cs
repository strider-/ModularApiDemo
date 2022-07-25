using FluentValidation;
using FluentValidation.Results;

namespace ModuleDemo.Modules.Todos.Models;

public class CreateTodoRequest
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public class Validator : AbstractValidator<CreateTodoRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("cannot be null or empty");
            RuleFor(x => x.Description).NotEmpty().WithMessage("cannot be null or empty");
        }

        protected override bool PreValidate(ValidationContext<CreateTodoRequest> context, ValidationResult result)
        {
            if (context.InstanceToValidate != null)
            {
                return true;
            }

            result.Errors.Add(new ValidationFailure("", "create todo request was missing or malformed"));
            return false;
        }
    }
}

