using MES.Data.Entities;
using MES.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.UserName == username);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}