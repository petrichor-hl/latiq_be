using System.Text.Json;
using LaTiQ.Application.DTOs;
using ApplicationException = LaTiQ.Application.Exceptions.ApplicationException;
namespace LaTiQ.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ApplicationException applicationException => applicationException.Code switch
            {
                ApiResultErrorCodes.ModelValidation => StatusCodes.Status400BadRequest,
                ApiResultErrorCodes.Conflict => StatusCodes.Status400BadRequest,
                ApiResultErrorCodes.NotFound => StatusCodes.Status404NotFound,
                ApiResultErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            },
            _ => StatusCodes.Status500InternalServerError,
        };

        var errorCode = exception switch
        {
            ApplicationException applicationException => applicationException.Code,
            _ => ApiResultErrorCodes.InternalServerError,
        };
        
        var errors = exception switch
        {
            _ => new ApiResultError[] { new ApiResultError(errorCode, exception.Message) }
        };

        // Tạo phản hồi lỗi
        var apiResult = ApiResult<object>.Failure(errors);
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true // Tùy chọn: thêm khoảng cách cho dễ đọc
        };
        
        var response = JsonSerializer.Serialize(apiResult, jsonOptions);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(response);
    }
}