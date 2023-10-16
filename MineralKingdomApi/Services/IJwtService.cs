using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}


