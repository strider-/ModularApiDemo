using FluentValidation;
using FluentValidation.Results;

namespace ModuleDemo.Modules.Todos.Models;

public class UpdateTodoRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool? Completed { get; set; }

    public class Validator : AbstractValidator<UpdateTodoRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .Unless(x => x.Title == null)
                .WithMessage("when supplied, cannot be an empty string");

            RuleFor(x => x.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .Unless(x => x.Description == null)
                .WithMessage("when supplied, cannot be an empty string");

            RuleFor(x => x)
                .Must(x => x.Title != null ||
                           x.Description != null ||
                           x.Completed != null)
                .WithMessage("you must update at least 1 field");
        }

        protected override bool PreValidate(ValidationContext<UpdateTodoRequest> context, ValidationResult result)
        {
            if (context.InstanceToValidate != null)
            {
                return true;
            }

            result.Errors.Add(new ValidationFailure("", "update todo request was missing or malformed"));
            return false;
        }
    }
}
