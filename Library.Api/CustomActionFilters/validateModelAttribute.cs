using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Api.CustomActionFilters
{
    public class validateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors?.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value!.Errors.Select(err => err.ErrorMessage).ToList()
                    });

                foreach (var error in errors)
                {
                    Console.WriteLine($"Model error on field '{error.Field}': {string.Join(", ", error.Errors)}");
                }

                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }

}
