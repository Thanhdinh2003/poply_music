using App.Common.Base;
using App.Domain.Entities;

namespace App.Application.Contracts.Infrastructure
{
    public interface IAuthorRepository
    {
        Task<Author> GetByIdAsync(Guid id);
        Task<List<Author>> GetByIdListAsync(List<Guid> idList);
        Task<BasePagination<Author>> SearchAuthor(Guid categoryId, string? name, string orderBy = "name", bool isAsc = false, int page = 1, int pageSize = 10);
    }
}
