using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.DTO.Response.Room;
using LaTiQ.Core.DTO.Response.Topic;
using LaTiQ.Core.DTO.Response.User;
using LaTiQ.Core.Entities.Room;
using LaTiQ.Core.Identity;
using LaTiQ.Infrastructure.DatabaseContext;
using LaTiQ.WebAPI.ServiceContracts;
using LaTiQ.WebAPI.Services;
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
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost("make-room")]
        public async Task<IActionResult> MakeRoom(MakeRoomRequest req)
        {
            ClaimsPrincipal principal = User;
            string email = principal.FindFirstValue(ClaimTypes.Email);

            RoomResponse? roomResponse = await _roomService.MakeRoom(email, req);

            if (roomResponse == null)
            {
                return BadRequest("Tạo phòng không thành công.");
            }
            else
            {
                return Ok(roomResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoom([FromQuery] string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                return BadRequest("Room ID is required.");
            }

            RoomResponse? roomResponse = _roomService.GetRoom(roomId);

            if (roomResponse == null)
            {
                return BadRequest("Không tìm thấy phòng.");
            }
            else
            {
                return Ok(roomResponse);
            }
        }
    }
}
