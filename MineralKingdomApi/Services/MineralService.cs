using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.DTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

namespace MineralKingdomApi.Services
{
    public class MineralService : IMineralService
    {
        private readonly IMineralRepository _mineralRepository;

        public MineralService(IMineralRepository mineralRepository)
        {
            _mineralRepository = mineralRepository;
        }

        public async Task<IEnumerable<MineralResponseDTO>> GetAllMineralsAsync()
        {
            var minerals = await _mineralRepository.GetAllMineralsAsync();
            return minerals.Select(m => new MineralResponseDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                Origin = m.Origin,
                CreatedAt = m.CreatedAt,
                ImageURL = m.ImageURL
            }).ToList();
        }

        public async Task<MineralResponseDTO?> GetMineralByIdAsync(int id)
        {
            var mineral = await _mineralRepository.GetMineralByIdAsync(id);
            return mineral == null ? null : new MineralResponseDTO
            {
                Id = mineral.Id,
                Name = mineral.Name,
                Description = mineral.Description,
                Price = mineral.Price,
                Origin = mineral.Origin,
                CreatedAt = mineral.CreatedAt
            };
        }

        public async Task<Mineral> CreateMineralAsync(CreateMineralDTO createMineralDTO)
        {
            var mineral = new Mineral
            {
                Name = createMineralDTO.Name,
                Description = createMineralDTO.Description,
                Price = (decimal)createMineralDTO.Price,
                Origin = createMineralDTO.Origin,
                CreatedAt = DateTime.UtcNow, // Assuming you want to set this at creation time
                ImageURL = createMineralDTO.ImageURL
            };

            await _mineralRepository.AddAsync(mineral);
            await _mineralRepository.SaveAsync(); // Ensure you have a method to save changes in your repository

            return mineral;
        }

        public async Task UpdateMineralAsync(int id, UpdateMineralDTO updateMineralDTO)
        {
            var mineral = await _mineralRepository.GetMineralByIdAsync(id);
            if (mineral != null)
            {
                mineral.Name = updateMineralDTO.Name ?? mineral.Name;
                mineral.Description = updateMineralDTO.Description ?? mineral.Description;
                mineral.Price = updateMineralDTO.Price ?? mineral.Price;  // Should work if Price is decimal?
                mineral.Origin = updateMineralDTO.Origin ?? mineral.Origin;
                await _mineralRepository.UpdateMineralAsync(mineral);
            }
        }


        public async Task DeleteMineralAsync(int id)
        {
            await _mineralRepository.DeleteMineralAsync(id);
        }

        public async Task AddAsync(Mineral mineral)
        {
            await _mineralRepository.AddAsync(mineral);
            await _mineralRepository.SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _mineralRepository.SaveAsync();
        }

        public MineralResponseDTO ConvertToMineralResponseDTO(Mineral mineral)
        {
            return new MineralResponseDTO
            {
                Id = mineral.Id,
                Name = mineral.Name,
                Description = mineral.Description,
                Price = mineral.Price,
                Origin = mineral.Origin,
                CreatedAt = mineral.CreatedAt,
                ImageURL = mineral.ImageURL
            };
        }
    }
}

