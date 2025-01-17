using LaTiQ.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LaTiQ.WebAPI.Filters;

public class ModelValidation : Attribute, IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(modelState => modelState.Errors)
                .Select(modelError => new ApiResultError(ApiResultErrorCodes.ModelValidation, modelError.ErrorMessage));

            context.Result = new BadRequestObjectResult(ApiResult<string>.Failure(errors));
        }

        await next();
    }
}