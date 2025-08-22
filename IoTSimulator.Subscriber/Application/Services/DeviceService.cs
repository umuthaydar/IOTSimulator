using AutoMapper;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;
using IoTSimulator.Subscriber.Domain.Enums;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Repositories;

namespace IoTSimulator.Subscriber.Application.Services;

public class DeviceService : IDeviceService
{
    private readonly DeviceRepository _deviceRepository;
    private readonly RoomRepository _roomRepository;
    private readonly IMapper _mapper;

    public DeviceService(DeviceRepository deviceRepository, RoomRepository roomRepository, IMapper mapper)
    {
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }



    public async Task<IoTDeviceDto> CreateDeviceAsync(CreateIoTDeviceDto createIoTDeviceDto)
    {
        ArgumentNullException.ThrowIfNull(createIoTDeviceDto);

        if (string.IsNullOrWhiteSpace(createIoTDeviceDto.Name))
            throw new ArgumentException("Device name is required.", nameof(createIoTDeviceDto));

        if (createIoTDeviceDto.RoomId == Guid.Empty)
            throw new ArgumentException("Room ID is required.", nameof(createIoTDeviceDto));

        var room = await _roomRepository.GetByIdAsync(createIoTDeviceDto.RoomId ?? Guid.Empty);
        if (room == null)
            throw new ArgumentException($"Room with ID {createIoTDeviceDto.RoomId} does not exist.", nameof(createIoTDeviceDto));

        var device = _mapper.Map<IoTDevice>(createIoTDeviceDto);
        device.IsActive = true;
        var createdDevice = await _deviceRepository.AddAsync(device);

        return _mapper.Map<IoTDeviceDto>(createdDevice);
    }

    public async Task<IoTDeviceDto> UpdateDeviceAsync(UpdateIoTDeviceDto updateIoTDeviceDto)
    {
        ArgumentNullException.ThrowIfNull(updateIoTDeviceDto);

        if (updateIoTDeviceDto.Id == Guid.Empty)
            throw new ArgumentException("Device ID is required for update.", nameof(updateIoTDeviceDto));

        var existingDevice = await _deviceRepository.GetByIdAsync(updateIoTDeviceDto.Id);
        if (existingDevice == null)
            throw new ArgumentException($"Device with ID {updateIoTDeviceDto.Id} does not exist.", nameof(updateIoTDeviceDto));

        if (string.IsNullOrWhiteSpace(updateIoTDeviceDto.Name))
            throw new ArgumentException("Device name is required.", nameof(updateIoTDeviceDto));

        if (updateIoTDeviceDto.RoomId == Guid.Empty)
            throw new ArgumentException("Room ID is required.", nameof(updateIoTDeviceDto));

        var room = await _roomRepository.GetByIdAsync(updateIoTDeviceDto.RoomId ?? Guid.Empty);
        if (room == null)
            throw new ArgumentException($"Room with ID {updateIoTDeviceDto.RoomId} does not exist.", nameof(updateIoTDeviceDto));

        _mapper.Map(updateIoTDeviceDto, existingDevice);
        var updatedDevice = await _deviceRepository.UpdateAsync(existingDevice);

        return _mapper.Map<IoTDeviceDto>(updatedDevice);
    }
    
    public async Task<bool> DeleteDeviceAsync(Guid id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null)
            return false;

        return await _deviceRepository.DeleteAsync(id);
    }

    public async Task<IoTDeviceDto?> GetDeviceByIdAsync(Guid id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        return device != null ? _mapper.Map<IoTDeviceDto>(device) : null;
    }

    public async Task<IEnumerable<IoTDeviceDto>> GetAllDevicesAsync()
    {
        var devices = await _deviceRepository.GetAllWithRoomAsync();
        return _mapper.Map<IEnumerable<IoTDeviceDto>>(devices);
    }

    public async Task<IEnumerable<IoTDeviceDto>> GetDevicesByRoomIdAsync(Guid roomId)
    {
        var devices = await _deviceRepository.GetByRoomIdWithRoomAsync(roomId);
        return _mapper.Map<IEnumerable<IoTDeviceDto>>(devices);
    }
}
