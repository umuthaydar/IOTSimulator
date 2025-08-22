using IoTSimulator.Subscriber.Application.DTOs;

namespace IoTSimulator.Subscriber.Application.Interfaces;

public interface IRoomService
{
    Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto);
    Task<RoomDto> UpdateRoomAsync(UpdateRoomDto updateRoomDto);
    Task<bool> DeleteRoomAsync(Guid id);
    Task<RoomDto?> GetRoomByIdAsync(Guid id);
    Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    Task<IEnumerable<RoomDto>> GetRoomsByHouseIdAsync(Guid houseId);
    Task<RoomWithDevicesDto?> GetRoomWithDevicesAsync(Guid id);
}
