using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class UserRepository(ServerDbContext context) : IUserRepository
{
    private readonly ServerDbContext Context = context;

    public async Task<List<User>> GetAllAsync()
    {
        return await Context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await Context.Users.FindAsync(id);
    }
}