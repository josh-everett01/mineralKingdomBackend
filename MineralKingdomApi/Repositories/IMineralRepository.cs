using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IMineralRepository
    {
        // Retrieves all minerals.
        Task<IEnumerable<Mineral>> GetAllMineralsAsync();

        // Retrieves a mineral by its ID.
        Task<Mineral?> GetMineralByIdAsync(int id);

        // Creates a new mineral.
        Task CreateMineralAsync(Mineral mineral);

        // Updates the specified mineral.
        Task UpdateMineralAsync(Mineral mineral);

        // Deletes a mineral by its ID.
        Task DeleteMineralAsync(int id);

        // Adds a new mineral.
        Task AddAsync(Mineral mineral);

        // Saves changes made to the repository.
        Task SaveAsync();

        // Updates a minerals status after purchase
        Task UpdateMineralStatusAsync(int mineralId, MineralStatus status);

    }


}

