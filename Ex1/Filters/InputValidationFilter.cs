using Ex1.Common.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Ex1.Filters
{
    public class InputValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var firstError = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault();

                if (firstError != null)
                {
                    var response = new BaseResponse
                    {
                        IsSuccess = false,
                        Message = firstError.ErrorMessage,
                        StatusCode = System.Net.HttpStatusCode.UnprocessableEntity
                    };

                    context.Result = new UnprocessableEntityObjectResult(response);

                }
            }
        }
    }
}
