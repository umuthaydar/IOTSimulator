using Microsoft.AspNetCore.Mvc;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;

namespace IoTSimulator.Subscriber.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom([FromBody] CreateRoomDto createRoomDto)
    {
        var createdRoom = await _roomService.CreateRoomAsync(createRoomDto);
        return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomDto>> UpdateRoom(Guid id, [FromBody] UpdateRoomDto updateRoomDto)
    {
        updateRoomDto.Id = id; // Ensure ID consistency
        var updatedRoom = await _roomService.UpdateRoomAsync(updateRoomDto);
        return Ok(updatedRoom);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoom(Guid id)
    {
        var deleted = await _roomService.DeleteRoomAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetRoom(Guid id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        return room != null ? Ok(room) : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms([FromQuery] Guid? houseId = null)
    {
        var rooms = houseId.HasValue 
            ? await _roomService.GetRoomsByHouseIdAsync(houseId.Value)
            : await _roomService.GetAllRoomsAsync();

        return Ok(rooms);
    }
}
