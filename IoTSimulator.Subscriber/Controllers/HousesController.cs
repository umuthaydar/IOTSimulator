using Microsoft.AspNetCore.Mvc;
using IoTSimulator.Subscriber.Application.DTOs;
using IoTSimulator.Subscriber.Application.Interfaces;

namespace IoTSimulator.Subscriber.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HousesController : ControllerBase
{
    private readonly IHouseService _houseService;

    public HousesController(IHouseService houseService)
    {
        _houseService = houseService ?? throw new ArgumentNullException(nameof(houseService));
    }

    [HttpPost]
    public async Task<ActionResult<HouseDto>> CreateHouse([FromBody] CreateHouseDto createHouseDto)
    {
        var createdHouse = await _houseService.CreateHouseAsync(createHouseDto);
        return CreatedAtAction(nameof(GetHouse), new { id = createdHouse.Id }, createdHouse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HouseDto>> UpdateHouse(Guid id, [FromBody] UpdateHouseDto updateHouseDto)
    {
        updateHouseDto.Id = id; // Ensure ID consistency
        var updatedHouse = await _houseService.UpdateHouseAsync(updateHouseDto);
        return Ok(updatedHouse);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHouse(Guid id)
    {
        var deleted = await _houseService.DeleteHouseAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HouseDto>> GetHouse(Guid id)
    {
        var house = await _houseService.GetHouseByIdAsync(id);
        return house != null ? Ok(house) : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HouseDto>>> GetHouses()
    {
        var houses = await _houseService.GetAllHousesAsync();
        return Ok(houses);
    }

    
}
