﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PolloPollo.Shared;

namespace PolloPollo.Repository
{
    public interface IUserRepository
    {
        Task<(DetailedUserDTO userDTO, string token)> Authenticate(string email, string password);
        Task<TokenDTO> CreateAsync(UserCreateDTO dto);
        Task<DetailedUserDTO> FindAsync(int userId);
        Task<string> StoreImageAsync(IFormFile file);
        Task<bool> UpdateAsync(UserUpdateDTO dto);
    }
}
