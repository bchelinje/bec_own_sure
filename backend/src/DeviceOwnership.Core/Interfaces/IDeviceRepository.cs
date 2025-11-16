using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Core.Interfaces;

public interface IDeviceRepository : IRepository<Device>
{
    Task<Device?> GetBySerialNumberHashAsync(string serialNumberHash, CancellationToken cancellationToken = default);
    Task<Device?> GetByVerificationCodeAsync(string verificationCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> GetUserDevicesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUserDeviceCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> GetStolenDevicesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> GetDevicesByCategoryAsync(string category, CancellationToken cancellationToken = default);
}
