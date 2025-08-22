using Microsoft.AspNetCore.Mvc;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;

namespace IoTSimulator.Subscriber.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
    }

    [HttpPost]
    public async Task<ActionResult<IoTDeviceDto>> CreateDevice([FromBody] CreateIoTDeviceDto createIoTDeviceDto)
    {
        var createdDevice = await _deviceService.CreateDeviceAsync(createIoTDeviceDto);
        return CreatedAtAction(nameof(GetDevice), new { id = createdDevice.Id }, createdDevice);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IoTDeviceDto>> UpdateDevice(Guid id, [FromBody] UpdateIoTDeviceDto updateIoTDeviceDto)
    {
        updateIoTDeviceDto.Id = id; // Ensure ID consistency
        var updatedDevice = await _deviceService.UpdateDeviceAsync(updateIoTDeviceDto);
        return Ok(updatedDevice);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDevice(Guid id)
    {
        var deleted = await _deviceService.DeleteDeviceAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IoTDeviceDto>> GetDevice(Guid id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        return device != null ? Ok(device) : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IoTDeviceDto>>> GetDevices([FromQuery] Guid? roomId = null)
    {
        var devices = roomId.HasValue 
            ? await _deviceService.GetDevicesByRoomIdAsync(roomId.Value)
            : await _deviceService.GetAllDevicesAsync();

        return Ok(devices);
    }
}
