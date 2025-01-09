using Microsoft.EntityFrameworkCore;
using SpotifyApi;
using SpotifyApi.Models.Authorization;
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
    
    public async Task<Registration?> GetRegistrationDataByUserIdAsync(int id)
    {
        var registrationModel = await Context.Users
            .Where(u => u.Id == id)
            .Select(u => new Registration()
            {
                Id = u.RegistrationId!.Value,
                AccessToken = u.Registration!.AccessToken,
                ExpiresDate = u.Registration!.ExpiresDate,
                RefreshToken = u.Registration!.RefreshToken
            })
            .FirstOrDefaultAsync();
        if (registrationModel is null)
            return null;
        if ((registrationModel.ExpiresDate - DateTime.UtcNow).TotalSeconds < 10)
        {
            var refreshResult = await SpotifyApi.SpotifyApi.RefreshTokenAsync(registrationModel.RefreshToken);
            return refreshResult.Result switch
            {
                ApiResult.RefreshTokenStillValid => registrationModel,
                ApiResult.Ok => await UpdateAccessToken(registrationModel, refreshResult.Data!),
                _ => null
            };
        }

        return registrationModel;
    }
    
    private async Task<Registration> UpdateAccessToken(Registration registrationModel,
        SpotifyAccessTokenDto newAccessToken)
    {
        var expiresDate = newAccessToken.ExpiresDate;
        registrationModel.AccessToken = newAccessToken.AccessToken;
        registrationModel.RefreshToken = newAccessToken.RefreshToken;
        registrationModel.ExpiresDate = newAccessToken.ExpiresDate;
        registrationModel.TokenType = newAccessToken.TokenType;
        await Context.SaveChangesAsync();
        return registrationModel;
    }
}