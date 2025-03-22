using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Api.CustomActionFilters
{
    public class validateModelAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid == false) {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
