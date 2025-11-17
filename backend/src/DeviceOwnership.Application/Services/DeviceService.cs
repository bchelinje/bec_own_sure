using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Enums;
using DeviceOwnership.Core.Interfaces;

namespace DeviceOwnership.Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;

    public DeviceService(
        IDeviceRepository deviceRepository,
        IUserRepository userRepository,
        IEncryptionService encryptionService)
    {
        _deviceRepository = deviceRepository;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
    }

    public async Task<Device> RegisterDeviceAsync(
        Guid userId,
        string serialNumber,
        string category,
        string? brand,
        string? model,
        CancellationToken cancellationToken = default)
    {
        // Check user exists
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Check device limit for free tier
        if (user.SubscriptionTier == SubscriptionTier.Free)
        {
            var deviceCount = await _deviceRepository.GetUserDeviceCountAsync(userId, cancellationToken);
            if (deviceCount >= 3)
            {
                throw new InvalidOperationException("Free tier limited to 3 devices. Please upgrade to premium.");
            }
        }

        // Check if serial number already registered
        var serialNumberHash = _encryptionService.Hash(serialNumber);
        var existingDevice = await _deviceRepository.GetBySerialNumberHashAsync(serialNumberHash, cancellationToken);
        if (existingDevice != null)
        {
            throw new InvalidOperationException("This device is already registered");
        }

        // Encrypt serial number
        var encryptedSerialNumber = await _encryptionService.EncryptAsync(serialNumber, cancellationToken);

        // Create device
        var device = new Device
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SerialNumberHash = serialNumberHash,
            SerialNumberEncrypted = encryptedSerialNumber,
            Category = category,
            Brand = brand,
            Model = model,
            Status = DeviceStatus.Active,
            VerificationCode = _encryptionService.GenerateRandomCode(6),
            RegisteredAt = DateTime.UtcNow
        };

        await _deviceRepository.AddAsync(device, cancellationToken);

        // Create initial ownership history
        var ownershipHistory = new OwnershipHistory
        {
            Id = Guid.NewGuid(),
            DeviceId = device.Id,
            FromUserId = null,
            ToUserId = userId,
            TransferMethod = "registered",
            TransferredAt = DateTime.UtcNow
        };

        return device;
    }

    public async Task<Device?> GetDeviceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _deviceRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetUserDevicesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _deviceRepository.GetUserDevicesAsync(userId, cancellationToken);
    }

    public async Task<Device?> CheckSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        var serialNumberHash = _encryptionService.Hash(serialNumber);
        return await _deviceRepository.GetBySerialNumberHashAsync(serialNumberHash, cancellationToken);
    }

    public async Task UpdateDeviceAsync(Device device, CancellationToken cancellationToken = default)
    {
        await _deviceRepository.UpdateAsync(device, cancellationToken);
    }

    public async Task DeleteDeviceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var device = await _deviceRepository.GetByIdAsync(id, cancellationToken);
        if (device == null)
        {
            throw new InvalidOperationException("Device not found");
        }

        device.Status = DeviceStatus.Deleted;
        device.DeletedAt = DateTime.UtcNow;
        await _deviceRepository.UpdateAsync(device, cancellationToken);
    }

    public async Task<bool> CanUserRegisterDeviceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return false;

        if (user.SubscriptionTier != SubscriptionTier.Free) return true;

        var deviceCount = await _deviceRepository.GetUserDeviceCountAsync(userId, cancellationToken);
        return deviceCount < 3;
    }
}
