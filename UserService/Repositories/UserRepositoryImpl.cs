using DBConnection;
using DBConnection.Entity;
using Microsoft.EntityFrameworkCore;

namespace UserService.Repositories
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepositoryImpl(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByLoginAsync(string login)
        {
            return await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<bool> IsLoginExistsAsync(string login)
        {
            return await _context.User
                .AsNoTracking()
                .AnyAsync(u => u.Login == login);
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await _context.User
                .AsNoTracking()
                .Where(u => u.RevokedOn == null)
                .OrderBy(u => u.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersOlderThanAsync(int age)
        {
            var minBirthDate = DateTime.UtcNow.AddYears(-age);
            return await _context.User
                .AsNoTracking()
                .Where(u => u.Birthday != null && u.Birthday <= minBirthDate)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(string login, string revokedBy)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user != null)
            {
                user.RevokedOn = DateTime.UtcNow;
                user.RevokedBy = revokedBy;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = revokedBy;
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreAsync(string login)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Login == login && u.RevokedOn != null);

            if (user != null)
            {
                user.RevokedOn = null;
                user.RevokedBy = null;
                user.ModifiedOn = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(User user)
        {
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}