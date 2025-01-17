using LaTiQ.Application.DTOs;

namespace LaTiQ.Application.Exceptions;

public class InvalidModelException : ApplicationException
{
    public InvalidModelException(string message) : base(ApiResultErrorCodes.ModelValidation, message)
    {
    }
}