using LaTiQ.Core.DTO.Response;
using LaTiQ.Core.Entities;
using LaTiQ.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.ServiceContracts
{
    public interface IJwtService
    {
        JwtToken CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string token);
    }
}
