using LaTiQ.Application.DTOs;

namespace LaTiQ.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message): base(ApiResultErrorCodes.NotFound, message)
    {
    }
}