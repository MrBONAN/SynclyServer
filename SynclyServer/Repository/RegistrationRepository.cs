using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models.Authorization;

namespace SynclyServer.Repository;

public class RegistrationRepository(ServerDbContext context) : IRegistrationRepository
{
    private readonly ServerDbContext Context = context;

    public async Task<Registration?> FindByIdAsync(int id)
    {
        return await Context.Registrations
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Registration?> FindAsync(string externalId, string platformName)
    {
        return await Context.Registrations
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ExternalId == externalId && r.PlatformName == platformName);
    }

    public async Task<Registration> CreateAsync(Registration registrationModel)
    {
        await Context.Registrations.AddAsync(registrationModel);
        await Context.SaveChangesAsync();
        return registrationModel;
    }

    public async Task<Registration?> UpdateAsync(int id, Registration updateRegistrationModel)
    {
        var registrationModel = await FindByIdAsync(id);
        if (registrationModel is null)
            return null;
        return await UpdateAsync(registrationModel, updateRegistrationModel);
    }

    public async Task<Registration?> UpdateAsync(Registration oldRegistrationModel,
        Registration updateRegistrationModel)
    {
        if (oldRegistrationModel.ExternalId != updateRegistrationModel.ExternalId ||
            oldRegistrationModel.PlatformName != updateRegistrationModel.PlatformName)
            return null;
        oldRegistrationModel.AccessToken = updateRegistrationModel.AccessToken;
        oldRegistrationModel.TokenType = updateRegistrationModel.TokenType;
        oldRegistrationModel.ExpiresDate = updateRegistrationModel.ExpiresDate;
        oldRegistrationModel.RefreshToken = updateRegistrationModel.RefreshToken;
        await Context.SaveChangesAsync();
        return oldRegistrationModel;
    }
}