using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
