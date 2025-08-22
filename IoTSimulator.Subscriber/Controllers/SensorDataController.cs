using Microsoft.AspNetCore.Mvc;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;
using IoTSimulator.Subscriber.Domain.Models;

namespace IoTSimulator.Subscriber.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorDataController : ControllerBase
{
    private readonly ISensorDataService _sensorDataService;

    public SensorDataController(ISensorDataService sensorDataService)
    {
        _sensorDataService = sensorDataService ?? throw new ArgumentNullException(nameof(sensorDataService));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetAllSensorData()
    {
        var sensorData = await _sensorDataService.GetAllSensorDataDtoAsync();
        return Ok(sensorData);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SensorDataDto>> GetSensorData(Guid id)
    {
        var sensorData = await _sensorDataService.GetSensorDataDtoByIdAsync(id);
        return sensorData != null ? Ok(sensorData) : NotFound();
    }

    [HttpGet("device/{deviceId}")]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensorDataByDevice(Guid deviceId)
    {
        var sensorData = await _sensorDataService.GetSensorDataDtoByDeviceIdAsync(deviceId);
        return Ok(sensorData);
    }

    [HttpGet("device/{deviceId}/range")]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensorDataByDateRange(
        Guid deviceId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var sensorData = await _sensorDataService.GetSensorDataDtoByDeviceIdAndDateRangeAsync(deviceId, startDate, endDate);
        return Ok(sensorData);
    }

    [HttpGet("device/{deviceId}/latest")]
    public async Task<ActionResult<SensorDataDto>> GetLatestSensorData(Guid deviceId)
    {
        var sensorData = await _sensorDataService.GetLatestSensorDataDtoByDeviceIdAsync(deviceId);
        return sensorData != null ? Ok(sensorData) : NotFound();
    }

    [HttpGet("device/{deviceId}/aggregate")]
    public async Task<ActionResult<SensorDataAggregate>> GetAggregatedSensorData(
        Guid deviceId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var aggregateData = await _sensorDataService.GetAggregatedDataAsync(deviceId, startDate, endDate);
        return Ok(aggregateData);
    }

    [HttpPost]
    public async Task<ActionResult<SensorDataDto>> CreateSensorData([FromBody] CreateSensorDataDto command)
    {
        var createdSensorData = await _sensorDataService.CreateSensorDataAsync(command);
        return CreatedAtAction(nameof(GetSensorData), new { id = createdSensorData.Id }, createdSensorData);
    }

    [HttpPost("from-mqtt")]
    public async Task<ActionResult<SensorData>> CreateSensorDataFromMqtt([FromBody] MqttSensorDataRequest mqttData)
    {
        var createdSensorData = await _sensorDataService.CreateSensorDataFromMqttAsync(
            mqttData.SensorId,
            mqttData.SensorName,
            mqttData.Location,
            mqttData.Temperature,
            mqttData.Humidity,
            mqttData.Timestamp != DateTime.MinValue ? mqttData.Timestamp : DateTime.UtcNow);

        return CreatedAtAction(nameof(GetSensorData), new { id = createdSensorData.Id }, createdSensorData);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SensorDataDto>> UpdateSensorData(Guid id, [FromBody] UpdateSensorDataDto command)
    {
        command.Id = id; // Ensure ID consistency
        var updatedSensorData = await _sensorDataService.UpdateSensorDataAsync(command);
        return Ok(updatedSensorData);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSensorData(Guid id)
    {
        var deleted = await _sensorDataService.DeleteSensorDataAsync(id);
        return deleted ? NoContent() : NotFound();
    }

        [HttpDelete("cleanup")]
    public async Task<ActionResult<int>> DeleteOldSensorData([FromQuery] DateTime olderThan)
    {
        var deletedCount = await _sensorDataService.DeleteOldSensorDataAsync(olderThan);
        return Ok(new { deletedCount, message = $"Deleted {deletedCount} sensor data records older than {olderThan:yyyy-MM-dd}." });
    }
}

public class MqttSensorDataRequest
{
    public string SensorId { get; set; } = string.Empty;
    public string? SensorName { get; set; }
    public string? Location { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public DateTime Timestamp { get; set; }
}
