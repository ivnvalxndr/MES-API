
using MES.Data.Entities;

namespace MES.Data.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UserExistsAsync(string username);
    Task UpdateLastLoginAsync(int userId);
}