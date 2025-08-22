using IoTSimulator.Subscriber.Application.DTOs;

namespace IoTSimulator.Subscriber.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
