using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.DTO.Request
{
    public class ConfirmEmailDTO
    {
        public string? Email { get; set; }
        public string? VerifyEmailToken { get; set; }
    }
}
