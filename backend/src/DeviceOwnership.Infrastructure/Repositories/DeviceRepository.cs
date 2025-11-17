using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Enums;
using DeviceOwnership.Core.Interfaces;
using DeviceOwnership.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceOwnership.Infrastructure.Repositories;

public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Device?> GetBySerialNumberHashAsync(string serialNumberHash, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.User)
            .Include(d => d.Photos)
            .Include(d => d.TheftReports.Where(t => t.Status == "active"))
            .FirstOrDefaultAsync(d => d.SerialNumberHash == serialNumberHash, cancellationToken);
    }

    public async Task<Device?> GetByVerificationCodeAsync(string verificationCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.VerificationCode == verificationCode, cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetUserDevicesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Photos.Where(p => p.IsPrimary))
            .Where(d => d.UserId == userId && d.DeletedAt == null)
            .OrderByDescending(d => d.RegisteredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUserDeviceCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(d => d.UserId == userId
                && d.DeletedAt == null
                && d.Status != DeviceStatus.Deleted,
                cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetStolenDevicesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.User)
            .Include(d => d.TheftReports)
            .Where(d => d.IsStolen && d.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetDevicesByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.Category.ToLower() == category.ToLower() && d.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.User)
            .Include(d => d.Photos)
            .Include(d => d.Documents)
            .Include(d => d.OwnershipHistory)
                .ThenInclude(o => o.FromUser)
            .Include(d => d.OwnershipHistory)
                .ThenInclude(o => o.ToUser)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}
