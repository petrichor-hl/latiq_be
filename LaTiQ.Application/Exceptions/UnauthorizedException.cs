using LaTiQ.Application.DTOs;

namespace LaTiQ.Application.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message) : base(ApiResultErrorCodes.Unauthorized, message) 
    {
    }
}