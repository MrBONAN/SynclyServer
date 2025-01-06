using SynclyServer.Models.Authorization;

namespace SynclyServer.Interfaces;

public interface IRegistrationRepository
{
    Task<Registration?> FindByIdAsync(int id);
    Task<Registration?> FindAsync(string externalId, string platformName);
    Task<Registration> CreateAsync(Registration registrationModel);
    Task<Registration?> UpdateAsync(int id, Registration updateRegistrationModel);
    Task<Registration?> UpdateAsync(Registration oldRegistrationModel, Registration updateRegistrationModel);
}