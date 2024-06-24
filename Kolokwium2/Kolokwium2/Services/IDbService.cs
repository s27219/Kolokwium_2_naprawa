using Kolokwium2.DTOs;
using Kolokwium2.Models;

namespace Kolokwium2.Services;

public interface IDbService
{
    public Task<GetCharacterDto?> GetCharacterInfo(int characterId);
    public Task<Character?> GetCharacterWithBackpackAsync(int characterId);
    public Task<List<Item>> GetItemsByIdsAsync(List<int> itemIds);
    public Task<bool> AddItemsToCharacterBackpackAsync(Character character, List<Item> items);
}