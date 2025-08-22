using IoTSimulator.Subscriber.Application.DTOs;

namespace IoTSimulator.Subscriber.Application.Interfaces;

public interface IDeviceService
{
    Task<IoTDeviceDto> CreateDeviceAsync(CreateIoTDeviceDto createIoTDeviceDto);
    Task<IoTDeviceDto> UpdateDeviceAsync(UpdateIoTDeviceDto updateIoTDeviceDto);
    Task<bool> DeleteDeviceAsync(Guid id);
    Task<IoTDeviceDto?> GetDeviceByIdAsync(Guid id);
    Task<IEnumerable<IoTDeviceDto>> GetAllDevicesAsync();
    Task<IEnumerable<IoTDeviceDto>> GetDevicesByRoomIdAsync(Guid roomId);
}
