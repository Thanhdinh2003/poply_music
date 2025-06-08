using App.Common.Base;
using App.Domain.Entities;

namespace App.Application.Contracts.Infrastructure
{
    public interface IMediaRepository
    {
        Task<Media> GetByIdAsync(Guid id);
        Task<Media> InsertOne(Media entity);
        Task<List<Media>> InsertMany(List<Media> entities);
        Task<BasePagination<Media>> GetPurchasedItemList(Guid userId, string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8);
        Task<BasePagination<Media>> GetAvailableItemList(Guid userId, string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8);
        Task<BasePagination<Media>> GetFavouriteItemList(Guid userId, string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8);
        Task<List<Media>> GetTrendingItemList(bool isAsc = true, int page = 1, int pageSize = 8);
        Task<List<Media>> GetSuggestedItemList(Guid userId, int pageSize = 8);
    }
}
