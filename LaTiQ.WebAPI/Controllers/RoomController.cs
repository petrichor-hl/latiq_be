using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.Entities.Room;
using LaTiQ.Core.Identity;
using LaTiQ.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HashSet<int> uniqueNumbers = new();

        public RoomController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("make-room")]
        public async Task<IActionResult> MakeRoom(MakeRoomRequest req)
        {
            ClaimsPrincipal principal = User;
            string email = principal.FindFirstValue(ClaimTypes.Email);

            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            int currentCount = uniqueNumbers.Count;
            int hashCode;
            do
            {
                hashCode = Guid.NewGuid().GetHashCode();
                if (hashCode < 0)
                {
                    hashCode *= -1;
                }
                uniqueNumbers.Add(hashCode);
            } while (uniqueNumbers.Count == currentCount);

            Room room = new()
            {
                RoomId = hashCode,
                OwnerId = user.Id,
                TopicId = req.TopicId,
                Capacity = req.Capacity,
                Round = req.Round,
                IsPublic = req.IsPublic,
            };

            return Ok(room);
        }
    }
}
