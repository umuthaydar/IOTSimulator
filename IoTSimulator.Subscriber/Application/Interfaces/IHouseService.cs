using IoTSimulator.Subscriber.Application.DTOs;

namespace IoTSimulator.Subscriber.Application.Interfaces;

public interface IHouseService
{
    Task<HouseDto> CreateHouseAsync(CreateHouseDto createHouseDto);
    Task<HouseDto> UpdateHouseAsync(UpdateHouseDto updateHouseDto);
    Task<bool> DeleteHouseAsync(Guid id);
    Task<HouseDto?> GetHouseByIdAsync(Guid id);
    Task<IEnumerable<HouseDto>> GetAllHousesAsync();
    Task<HouseWithRoomsDto?> GetHouseWithRoomsAsync(Guid id);
    
}
