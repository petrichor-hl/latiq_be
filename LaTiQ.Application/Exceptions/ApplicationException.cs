using LaTiQ.Application.DTOs;

namespace LaTiQ.Application.Exceptions;

public class ApplicationException : Exception
{
    public ApiResultErrorCodes Code { get; set; }
    public ApplicationException(ApiResultErrorCodes code, string message) : base(message)
    {
        Code = code;
    }
}