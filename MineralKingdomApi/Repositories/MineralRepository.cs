using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

public class MineralRepository : IMineralRepository
{
    private readonly MineralKingdomContext _dbContext;

    public MineralRepository(MineralKingdomContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<Mineral>> GetAllMineralsAsync()
    {
        return await _dbContext.Minerals.ToListAsync();
    }

    public async Task<Mineral?> GetMineralByIdAsync(int id)
    {
        return await _dbContext.Minerals.FindAsync(id);
    }

    public async Task CreateMineralAsync(Mineral mineral)
    {
        await _dbContext.Minerals.AddAsync(mineral);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMineralAsync(Mineral mineral)
    {
        _dbContext.Minerals.Update(mineral);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteMineralAsync(int id)
    {
        var mineral = await GetMineralByIdAsync(id);
        if (mineral != null)
        {
            _dbContext.Minerals.Remove(mineral);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task AddAsync(Mineral mineral)
    {
        await _dbContext.Minerals.AddAsync(mineral);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateMineralStatusAsync(int mineralId, MineralStatus status)
    {
        var mineral = await _dbContext.Minerals.FindAsync(mineralId);
        if (mineral != null)
        {
            mineral.Status = status;
            _dbContext.Minerals.Update(mineral);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Mineral with ID {mineralId} not found.");
        }
    }

}

