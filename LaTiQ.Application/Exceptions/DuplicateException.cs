using LaTiQ.Application.DTOs;

namespace LaTiQ.Application.Exceptions;

public class DuplicateException : ApplicationException
{
    public DuplicateException(string message): base(ApiResultErrorCodes.Conflict, message)
    {
    }
}