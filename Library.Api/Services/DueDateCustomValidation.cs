using Library.Api.Controllers;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Services
{
    public class DueDateCustomValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not DateOnly date)
                return false;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var isValid = date > today;

            if (!isValid)
            {
                Console.WriteLine($"Due date {date} is not valid. Must be after today: {today}.");
            }

            return isValid;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be at least one day after today.";
        }
    }

}

