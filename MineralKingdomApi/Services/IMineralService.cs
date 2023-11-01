using System;
using MineralKingdomApi.DTOs;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Services
{
    public interface IMineralService
    {
        Task AddAsync(Mineral mineral);
        MineralResponseDTO ConvertToMineralResponseDTO(Mineral mineral);
        Task<Mineral> CreateMineralAsync(CreateMineralDTO createMineralDTO);
        Task DeleteMineralAsync(int id);
        Task<IEnumerable<MineralResponseDTO>> GetAllMineralsAsync();
        Task<MineralResponseDTO?> GetMineralByIdAsync(int id);
        Task SaveAsync();
        Task UpdateMineralAsync(int id, UpdateMineralDTO updateMineralDTO);
        Task UpdateMineralStatusAsync(int mineralId, MineralStatus status);
    }

}

