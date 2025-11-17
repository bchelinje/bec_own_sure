using DeviceOwnership.Core.Entities;

namespace DeviceOwnership.Core.Interfaces;

public interface IMarketplaceRepository : IRepository<MarketplaceListing>
{
    Task<IEnumerable<MarketplaceListing>> GetActiveListingsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MarketplaceListing>> GetUserListingsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MarketplaceListing>> GetFilteredListingsAsync(
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? condition = null,
        string? location = null,
        CancellationToken cancellationToken = default);
}
