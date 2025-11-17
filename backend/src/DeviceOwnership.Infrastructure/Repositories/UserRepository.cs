using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Interfaces;
using DeviceOwnership.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceOwnership.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }
}
