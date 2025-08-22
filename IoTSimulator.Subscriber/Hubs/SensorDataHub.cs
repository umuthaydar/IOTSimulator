using Microsoft.AspNetCore.SignalR;

namespace IoTSimulator.Subscriber.Hubs
{
    public class SensorDataHub : Hub
    {
        public async Task JoinRoomGroup(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Room-{roomId}");
        }

        public async Task LeaveRoomGroup(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Room-{roomId}");
        }

        public async Task JoinHouseGroup(string houseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"House-{houseId}");
        }

        public async Task LeaveHouseGroup(string houseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"House-{houseId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }

    public static class SensorDataHubExtensions
    {
        public static async Task SendSensorDataToRoom(this IHubContext<SensorDataHub> hubContext, 
            string roomId, object sensorData)
        {
            Console.WriteLine($"[DEBUG] Sending sensor data to Room group: Room-{roomId}");
            await hubContext.Clients.Group($"Room-{roomId}").SendAsync("SensorDataUpdate", sensorData);
        }

        public static async Task SendSensorDataToHouse(this IHubContext<SensorDataHub> hubContext, 
            string houseId, object sensorData)
        {
            Console.WriteLine($"[DEBUG] Sending sensor data to House group: House-{houseId}");
            await hubContext.Clients.Group($"House-{houseId}").SendAsync("SensorDataUpdate", sensorData);
        }

        public static async Task SendDeviceStatusUpdate(this IHubContext<SensorDataHub> hubContext, 
            string roomId, object deviceData)
        {
            await hubContext.Clients.Group($"Room-{roomId}").SendAsync("DeviceStatusUpdate", deviceData);
        }
    }
}
