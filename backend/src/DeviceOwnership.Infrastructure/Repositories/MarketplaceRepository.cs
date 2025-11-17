using DeviceOwnership.Core.Entities;
using DeviceOwnership.Core.Interfaces;
using DeviceOwnership.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceOwnership.Infrastructure.Repositories;

public class MarketplaceRepository : Repository<MarketplaceListing>, IMarketplaceRepository
{
    public MarketplaceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MarketplaceListing>> GetActiveListingsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MarketplaceListings
            .Where(l => l.Status == "active")
            .Include(l => l.Device)
            .Include(l => l.Seller)
            .OrderByDescending(l => l.ListedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MarketplaceListing>> GetUserListingsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.MarketplaceListings
            .Where(l => l.SellerId == userId)
            .Include(l => l.Device)
            .OrderByDescending(l => l.ListedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MarketplaceListing>> GetFilteredListingsAsync(
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? condition = null,
        string? location = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.MarketplaceListings
            .Where(l => l.Status == "active")
            .Include(l => l.Device)
            .Include(l => l.Seller)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(l => l.Category == category);

        if (minPrice.HasValue)
            query = query.Where(l => l.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(l => l.Price <= maxPrice.Value);

        if (!string.IsNullOrEmpty(condition))
            query = query.Where(l => l.Condition == condition);

        if (!string.IsNullOrEmpty(location))
            query = query.Where(l => l.Location != null && l.Location.Contains(location));

        return await query.OrderByDescending(l => l.ListedAt).ToListAsync(cancellationToken);
    }
}
