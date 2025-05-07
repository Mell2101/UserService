using System;
using DBConnection.Entity;

namespace UserService.Repositories;

public interface IUserRepository
{
    Task<User> GetByLoginAsync(string login);
    Task<bool> IsLoginExistsAsync(string login);
    Task<List<User>> GetActiveUsersAsync();
    Task<List<User>> GetUsersOlderThanAsync(int age);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task SoftDeleteAsync(string login, string revokedBy);
    Task RestoreAsync(string login);
    Task DeleteAsync(User user);
}
