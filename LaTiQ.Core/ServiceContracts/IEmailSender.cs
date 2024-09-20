using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.ServiceContracts
{
    public interface IEmailSender
    {
        Task SendMailAsync(string email, string subject, string message);
    }
}
