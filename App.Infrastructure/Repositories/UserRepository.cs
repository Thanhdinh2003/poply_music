using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User, DatabaseContext>, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly DatabaseContext _context;
        public UserRepository(DatabaseContext context, ILogger<UserRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> Login(string username, string password)
        {
            var userLogin = await _context.Users.FirstOrDefaultAsync(p => p.Username == username.Trim() && p.Password == password);
            if (userLogin == null)
            {
                _logger.LogError($"user with username = ${username} login failed.");
                throw new ArgumentNullException($"Failed to login with username: {username}");
            }
            return userLogin;
        }
    }
}
