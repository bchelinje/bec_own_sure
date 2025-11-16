using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Core.Interfaces;

public interface IDeviceService
{
    Task<Device> RegisterDeviceAsync(Guid userId, string serialNumber, string category, string? brand, string? model, CancellationToken cancellationToken = default);
    Task<Device?> GetDeviceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> GetUserDevicesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Device?> CheckSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task UpdateDeviceAsync(Device device, CancellationToken cancellationToken = default);
    Task DeleteDeviceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> CanUserRegisterDeviceAsync(Guid userId, CancellationToken cancellationToken = default);
}
