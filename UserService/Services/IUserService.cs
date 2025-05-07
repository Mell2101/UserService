using DBConnection.Entity;
using UserService.Dto;

namespace UserService.Services
{
    public interface IUserService
    {
        // 1. Создание пользователя
        Task<Result<User>> CreateUserAsync(UserCreateDto dto, string adminLogin);
        
        // 2. Обновление данных пользователя
        Task<Result> UpdateUserAsync(string login, UserUpdateDto dto, string currentUserLogin);
        
        // 3. Смена пароля
        Task<Result> ChangePasswordAsync(string login, ChangePasswordDto dto, string currentUserLogin);
        
        // 4. Смена логина
        Task<Result> ChangeLoginAsync(string oldLogin, ChangeLoginDto dto, string currentUserLogin);
        
        // 5. Получение списка активных пользователей
        Task<Result<IEnumerable<UserResponseDto>>> GetActiveUsersAsync(string adminLogin);
        
        // 6. Получение пользователя по логину (для админа)
        Task<Result<UserResponseDto>> GetUserByLoginAsync(string login, string adminLogin);
        
        // 7. Получение информации о себе
        Task<Result<UserResponseDto>> GetSelfAsync(string login, string password);
        
        // 8. Получение пользователей старше возраста
        Task<Result<IEnumerable<UserResponseDto>>> GetUsersOlderThanAsync(int age, string adminLogin);
        
        // 9. Удаление пользователя
        Task<Result> DeleteUserAsync(string login, bool softDelete, string adminLogin);
        
        // 10. Восстановление пользователя
        Task<Result> RestoreUserAsync(string login, string adminLogin);
    }
}