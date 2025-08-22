using AutoMapper;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Repositories;

namespace IoTSimulator.Subscriber.Application.Services;

public class RoomService : IRoomService
{
    private readonly RoomRepository _roomRepository;
    private readonly HouseRepository _houseRepository;
    private readonly IMapper _mapper;

    public RoomService(RoomRepository roomRepository, HouseRepository houseRepository, IMapper mapper)
    {
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        _houseRepository = houseRepository ?? throw new ArgumentNullException(nameof(houseRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto)
    {
        ArgumentNullException.ThrowIfNull(createRoomDto);

        if (string.IsNullOrWhiteSpace(createRoomDto.Name))
            throw new ArgumentException("Room name is required.", nameof(createRoomDto));

        if (createRoomDto.HouseId == Guid.Empty)
            throw new ArgumentException("House ID is required.", nameof(createRoomDto));

        var house = await _houseRepository.GetByIdAsync(createRoomDto.HouseId);
        if (house == null)
            throw new ArgumentException($"House with ID {createRoomDto.HouseId} does not exist.", nameof(createRoomDto));

        var existingRooms = await _roomRepository.GetByHouseIdAsync(createRoomDto.HouseId);
        var duplicateRoom = existingRooms.FirstOrDefault(r => r.Name.Equals(createRoomDto.Name, StringComparison.OrdinalIgnoreCase));
        if (duplicateRoom != null)
            throw new ArgumentException($"A room with name '{createRoomDto.Name}' already exists in this house.", nameof(createRoomDto));

        var room = _mapper.Map<Room>(createRoomDto);
        var createdRoom = await _roomRepository.AddAsync(room);

        return _mapper.Map<RoomDto>(createdRoom);
    }

    public async Task<RoomDto> UpdateRoomAsync(UpdateRoomDto updateRoomDto)
    {
        ArgumentNullException.ThrowIfNull(updateRoomDto);

        if (updateRoomDto.Id == Guid.Empty)
            throw new ArgumentException("Room ID is required for update.", nameof(updateRoomDto));

        var existingRoom = await _roomRepository.GetByIdAsync(updateRoomDto.Id);
        if (existingRoom == null)
            throw new ArgumentException($"Room with ID {updateRoomDto.Id} does not exist.", nameof(updateRoomDto));

        if (string.IsNullOrWhiteSpace(updateRoomDto.Name))
            throw new ArgumentException("Room name is required.", nameof(updateRoomDto));

        if (updateRoomDto.HouseId == Guid.Empty)
            throw new ArgumentException("House ID is required.", nameof(updateRoomDto));

        var house = await _houseRepository.GetByIdAsync(updateRoomDto.HouseId);
        if (house == null)
            throw new ArgumentException($"House with ID {updateRoomDto.HouseId} does not exist.", nameof(updateRoomDto));

        var existingRooms = await _roomRepository.GetByHouseIdAsync(updateRoomDto.HouseId);
        var duplicateRoom = existingRooms.FirstOrDefault(r =>
            r.Name.Equals(updateRoomDto.Name, StringComparison.OrdinalIgnoreCase) && r.Id != updateRoomDto.Id);
        if (duplicateRoom != null)
            throw new ArgumentException($"A room with name '{updateRoomDto.Name}' already exists in this house.", nameof(updateRoomDto));

        _mapper.Map(updateRoomDto, existingRoom);
        var updatedRoom = await _roomRepository.UpdateAsync(existingRoom);

        return _mapper.Map<RoomDto>(updatedRoom);
    }
    
    public async Task<bool> DeleteRoomAsync(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
            return false;

        var roomWithDevices = await _roomRepository.GetWithDevicesAsync(id);
        if (roomWithDevices?.Devices?.Any() == true)
            throw new InvalidOperationException($"Cannot delete room '{room.Name}' because it contains {roomWithDevices.Devices.Count()} devices. Delete the devices first or use soft delete.");

        return await _roomRepository.DeleteAsync(id);
    }

    public async Task<RoomDto?> GetRoomByIdAsync(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        return room != null ? _mapper.Map<RoomDto>(room) : null;
    }

    public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepository.GetAllWithHouseAsync(); // Include House
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<IEnumerable<RoomDto>> GetRoomsByHouseIdAsync(Guid houseId)
    {
        var rooms = await _roomRepository.GetByHouseIdAsync(houseId);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<RoomWithDevicesDto?> GetRoomWithDevicesAsync(Guid id)
    {
        var room = await _roomRepository.GetWithDevicesAsync(id);
        return room != null ? _mapper.Map<RoomWithDevicesDto>(room) : null;
    }
}
