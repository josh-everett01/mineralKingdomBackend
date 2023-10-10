using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IMineralRepository
    {
        Task<IEnumerable<Mineral>> GetAllMineralsAsync();
        Task<Mineral?> GetMineralByIdAsync(int id);
        Task CreateMineralAsync(Mineral mineral);
        Task UpdateMineralAsync(Mineral mineral);
        Task DeleteMineralAsync(int id);
        Task AddAsync(Mineral mineral);
        Task SaveAsync();
    }

}

