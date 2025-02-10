using LaTiQ.WebAPI.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using LaTiQ.Application.DTOs;
using LaTiQ.Application.DTOs.Room.Req;
using LaTiQ.Application.DTOs.Room.Res;

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
        public async Task<IActionResult> MakeRoom(MakeRoomRequest makeRoomRequest)
        {
            return Ok(ApiResult<RoomResponse>.Success(await _roomService.MakeRoom(makeRoomRequest)));
        }

        [HttpGet("{roomId}")]
        public IActionResult GetRoom(string roomId)
        {
            return Ok(ApiResult<RoomResponse>.Success(_roomService.GetRoom(roomId)));
        }
        
        [HttpGet("get-public-rooms")]
        public IActionResult GetPublicRooms()
        {
            return Ok(ApiResult<List<RoomResponse>>.Success(_roomService.GetPublicRooms()));
        }
    }
}
