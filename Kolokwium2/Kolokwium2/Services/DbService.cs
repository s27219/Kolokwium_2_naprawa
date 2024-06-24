using Kolokwium2.Data;
using Kolokwium2.DTOs;
using Kolokwium2.Models;
using Microsoft.EntityFrameworkCore;

namespace Kolokwium2.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;
    public DbService(DatabaseContext context)
    {
        _context = context;
    }
    public async Task<GetCharacterDto?> GetCharacterInfo(int characterId)
    {
        var character = await _context.Characters
            .Include(c => c.CharacterTitles)
            .ThenInclude(ct => ct.Title)
            .Include(c => c.Backpacks)
            .ThenInclude(b => b.Item)
            .Where(c => c.Id == characterId)
            .FirstOrDefaultAsync();
            
        if (character == null)
        {
            return null;
        }

        return new GetCharacterDto
        {
            FirstName = character.FirstName,
            LastName = character.LastName,
            CurrentWeight = character.CurrentWeight,
            MaxWeight = character.MaxWeight,
            BackpackItems = character.Backpacks.Select(b => new GetBackpackItemsDto
            {
                ItemName = b.Item.Name,
                ItemWeight = b.Item.Weight,
                Amount = b.Amount
            }).ToList(),
            Titles = character.CharacterTitles.Select(ct => new GetTitleDto()
            {
                Title = ct.Title.Name,
                AquiredAt = ct.AquiredAt
            }).ToList()

        };
    }
    
    public async Task<Character?> GetCharacterWithBackpackAsync(int characterId)
    {
        return await _context.Characters
            .Include(c => c.Backpacks)
            .ThenInclude(b => b.Item)
            .FirstOrDefaultAsync(c => c.Id == characterId);
    }

    public async Task<List<Item>> GetItemsByIdsAsync(List<int> itemIds)
    {
        return await _context.Items.Where(i => itemIds.Contains(i.Id)).ToListAsync();
    }

    public async Task<bool> AddItemsToCharacterBackpackAsync(Character character, List<Item> items)
    {
        int totalNewWeight = items.Sum(i => i.Weight);
        if (character.CurrentWeight + totalNewWeight > character.MaxWeight)
        {
            return false; // Not enough capacity
        }

        foreach (var item in items)
        {
            var backpack = character.Backpacks.FirstOrDefault(b => b.ItemId == item.Id);
            if (backpack == null)
            {
                character.Backpacks.Add(new Backpack { CharacterId = character.Id, ItemId = item.Id, Amount = 1 });
            }
            else
            {
                backpack.Amount += 1;
            }
            character.CurrentWeight += item.Weight;
        }

        await _context.SaveChangesAsync();
        return true; // Items added successfully
    }

    
}