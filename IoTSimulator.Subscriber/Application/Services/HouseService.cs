using AutoMapper;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;
using IoTSimulator.Subscriber.Domain.Models;
using IoTSimulator.Subscriber.Infrastructure.Repositories;

namespace IoTSimulator.Subscriber.Application.Services;

public class HouseService : IHouseService
{
    private readonly HouseRepository _houseRepository;
    private readonly IMapper _mapper;

    public HouseService(HouseRepository houseRepository, IMapper mapper)
    {
        _houseRepository = houseRepository ?? throw new ArgumentNullException(nameof(houseRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<HouseDto> CreateHouseAsync(CreateHouseDto createHouseDto)
    {
        ArgumentNullException.ThrowIfNull(createHouseDto);

        if (string.IsNullOrWhiteSpace(createHouseDto.Name))
            throw new ArgumentException("House name is required.", nameof(createHouseDto));

        if (string.IsNullOrWhiteSpace(createHouseDto.Address))
            throw new ArgumentException("House address is required.", nameof(createHouseDto));

        var house = _mapper.Map<House>(createHouseDto);
        var createdHouse = await _houseRepository.AddAsync(house);

        return _mapper.Map<HouseDto>(createdHouse);
    }

    public async Task<HouseDto> UpdateHouseAsync(UpdateHouseDto updateHouseDto)
    {
        ArgumentNullException.ThrowIfNull(updateHouseDto);

        if (string.IsNullOrWhiteSpace(updateHouseDto.Name))
            throw new ArgumentException("House name is required.", nameof(updateHouseDto));

        if (string.IsNullOrWhiteSpace(updateHouseDto.Address))
            throw new ArgumentException("House address is required.", nameof(updateHouseDto));

        var existingHouse = await _houseRepository.GetByIdAsync(updateHouseDto.Id);
        if (existingHouse == null)
            throw new InvalidOperationException($"House with ID {updateHouseDto.Id} not found.");

        _mapper.Map(updateHouseDto, existingHouse);
        var updatedHouse = await _houseRepository.UpdateAsync(existingHouse);

        return _mapper.Map<HouseDto>(updatedHouse);
    }
    
    public async Task<bool> DeleteHouseAsync(Guid id)
    {
        return await _houseRepository.DeleteAsync(id);
    }

    public async Task<HouseDto?> GetHouseByIdAsync(Guid id)
    {
        var house = await _houseRepository.GetByIdAsync(id);
        return house != null ? _mapper.Map<HouseDto>(house) : null;
    }

    public async Task<IEnumerable<HouseDto>> GetAllHousesAsync()
    {
        var houses = await _houseRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<HouseDto>>(houses);
    }

    public async Task<HouseWithRoomsDto?> GetHouseWithRoomsAsync(Guid id)
    {
        var house = await _houseRepository.GetWithRoomsAsync(id);
        return house != null ? _mapper.Map<HouseWithRoomsDto>(house) : null;
    }
}
