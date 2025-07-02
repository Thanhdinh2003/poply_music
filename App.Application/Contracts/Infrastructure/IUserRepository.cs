using App.Domain.Entities;

namespace App.Application.Contracts.Infrastructure
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> InsertOne(User entity);
        Task<User> Login(string username, string password);
    }
}
