using System.Transactions;
using Kolokwium2.DTOs;
using Kolokwium2.Models;
using Kolokwium2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CharactersController : ControllerBase
{
    private readonly IDbService _dbService;
    public CharactersController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("{characterId}")]
    public async Task<ActionResult<GetCharacterDto>> GetPatient(int characterId)
    {
        var character = await _dbService.GetCharacterInfo(characterId);

        if (character == null)
            return NotFound("Character not found");

        return Ok(character);
    }
    
    [HttpPost("{characterId}/backpacks")]
    public async Task<IActionResult> AddItemsToBackpack(int characterId, [FromBody] AddItemsDto addItemsDto)
    {
        var character = await _dbService.GetCharacterWithBackpackAsync(characterId);
        if (character == null)
            return NotFound("Character not found");

        var items = await _dbService.GetItemsByIdsAsync(addItemsDto.ItemIds);
        if (items.Count != addItemsDto.ItemIds.Count)
            return BadRequest("Some items do not exist");

        bool success = await _dbService.AddItemsToCharacterBackpackAsync(character, items);
        if (!success)
            return BadRequest("Character cannot carry more weight");

        var response = character.Backpacks.Select(b => new
        {
            b.Amount,
            b.ItemId,
            b.CharacterId
        }).ToList();

        return Ok(response);
    }
}