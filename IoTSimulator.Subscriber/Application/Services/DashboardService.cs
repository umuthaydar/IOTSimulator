using AutoMapper;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;
using IoTSimulator.Subscriber.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IoTSimulator.Subscriber.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly HouseRepository _houseRepository;
    private readonly RoomRepository _roomRepository;
    private readonly DeviceRepository _deviceRepository;
    private readonly SensorDataRepository _sensorDataRepository;
    private readonly IMapper _mapper;

    public DashboardService(
        HouseRepository houseRepository,
        RoomRepository roomRepository,
        DeviceRepository deviceRepository,
        SensorDataRepository sensorDataRepository,
        IMapper mapper)
    {
        _houseRepository = houseRepository ?? throw new ArgumentNullException(nameof(houseRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _sensorDataRepository = sensorDataRepository ?? throw new ArgumentNullException(nameof(sensorDataRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var houses = await _houseRepository.GetAllAsync();
        var rooms = await _roomRepository.GetAllAsync();
        var devices = await _deviceRepository.GetAllAsync();

        var totalHouses = houses.Count();
        var totalRooms = rooms.Count();
        var totalDevices = devices.Count();
        var activeDevices = devices.Count(d => d.IsActive);
        var inactiveDevices = totalDevices - activeDevices;
        var activeDevicePercentage = totalDevices > 0 ? (double)activeDevices / totalDevices * 100 : 0;

        return new DashboardStatsDto
        {
            TotalHouses = totalHouses,
            TotalRooms = totalRooms,
            TotalDevices = totalDevices,
            ActiveDevices = activeDevices,
            InactiveDevices = inactiveDevices,
            ActiveDevicePercentage = Math.Round(activeDevicePercentage, 2),
            LastUpdated = DateTime.UtcNow
        };
    }
}
