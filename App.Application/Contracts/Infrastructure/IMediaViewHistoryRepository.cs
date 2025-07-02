using App.Common.Base;
using App.Domain.Entities;

namespace App.Application.Contracts.Infrastructure
{
    public interface IMediaViewHistoryRepository
    {
        Task<MediaViewHistory> GetByIdAsync(Guid id);
        Task<MediaViewHistory> InsertOne(MediaViewHistory entity);
        Task<BasePagination<MediaViewHistory>> Search(Guid userId, Guid mediaId, Guid categoryId, Guid authorId,
            string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8);
    }
}
