using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

public class MineralRepository : IMineralRepository
{
    private readonly MineralKingdomContext _mineralService;

    public MineralRepository(MineralKingdomContext context)
    {
        _mineralService = context;
    }

    public async Task<IEnumerable<Mineral>> GetAllMineralsAsync()
    {
        return await _mineralService.Minerals.ToListAsync();
    }

    public async Task<Mineral?> GetMineralByIdAsync(int id)
    {
        return await _mineralService.Minerals.FindAsync(id);
    }

    public async Task CreateMineralAsync(Mineral mineral)
    {
        await _mineralService.Minerals.AddAsync(mineral);
        await _mineralService.SaveChangesAsync();
    }

    public async Task UpdateMineralAsync(Mineral mineral)
    {
        _mineralService.Minerals.Update(mineral);
        await _mineralService.SaveChangesAsync();
    }

    public async Task DeleteMineralAsync(int id)
    {
        var mineral = await GetMineralByIdAsync(id);
        if (mineral != null)
        {
            _mineralService.Minerals.Remove(mineral);
            await _mineralService.SaveChangesAsync();
        }
    }

    public async Task AddAsync(Mineral mineral)
    {
        await _mineralService.Minerals.AddAsync(mineral);
    }

    public async Task SaveAsync()
    {
        await _mineralService.SaveChangesAsync();
    }
}

