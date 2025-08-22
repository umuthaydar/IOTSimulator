using System.ComponentModel.DataAnnotations;

namespace IoTSimulator.Subscriber.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalHouses { get; set; }
    public int TotalRooms { get; set; }
    public int TotalDevices { get; set; }
    public int ActiveDevices { get; set; }
    public int InactiveDevices { get; set; }
    public double ActiveDevicePercentage { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class DashboardSummaryDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public IEnumerable<HouseStatsDto> HouseStats { get; set; } = new List<HouseStatsDto>();
    public IEnumerable<RecentActivityDto> RecentActivity { get; set; } = new List<RecentActivityDto>();
}

public class HouseStatsDto
{
    public Guid HouseId { get; set; }
    public string HouseName { get; set; } = string.Empty;
    public int RoomCount { get; set; }
    public int DeviceCount { get; set; }
    public int ActiveDeviceCount { get; set; }
    public DateTime? LastSensorUpdate { get; set; }
}

public class RecentActivityDto
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty; // "SensorUpdate", "DeviceAdded", etc.
    public string Description { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string HouseName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
