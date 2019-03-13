﻿using PolloPollo.Shared;
using System.Threading.Tasks;

namespace PolloPollo.Repository
{
    public interface IUserRepository
    {
        string Authenticate(string email, string password);

        Task<int> CreateAsync(UserCreateDTO dto);

        Task<UserDTO> FindAsync(int userId);
    }
}
