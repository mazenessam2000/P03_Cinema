using System.ComponentModel.DataAnnotations;

namespace P03_Cinema.Validations;

public class NotInFutureAttribute : ValidationAttribute
{
    public NotInFutureAttribute()
    {
        ErrorMessage = "Date cannot be in the future.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date && date > DateTime.Today)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}