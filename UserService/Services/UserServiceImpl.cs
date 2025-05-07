using DBConnection.Entity;
using UserService.Dto;
using UserService.Repositories;

namespace UserService.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserServiceImpl> _logger;

        public UserServiceImpl(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger<UserServiceImpl> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result<User>> CreateUserAsync(UserCreateDto dto, string adminLogin)
        {
            try
            {
                var admin = await _userRepository.GetByLoginAsync(adminLogin);
                if (admin == null || !admin.Admin || admin.RevokedOn != null)
                    return Result.Fail<User>("Только активные администраторы могут создавать пользователей");

                if (await _userRepository.IsLoginExistsAsync(dto.Login))
                    return Result.Fail<User>("Логин уже существует");

                var user = new User
                {
                    Login = dto.Login,
                    Password = _passwordHasher.Hash(dto.Password),
                    Name = dto.Name,
                    Gender = dto.Gender,
                    Birthday = dto.Birthday,
                    Admin = dto.IsAdmin,
                    CreatedBy = adminLogin,
                    ModifiedBy = adminLogin,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                return Result.Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя");
                return Result.Fail<User>("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result> UpdateUserAsync(string login, UserUpdateDto dto, string currentUserLogin)
        {
            try
            {

                var currentUser = await _userRepository.GetByLoginAsync(currentUserLogin);
                if (currentUser == null)
                    return Result.Fail("Пользователь не найден");

                var userToUpdate = await _userRepository.GetByLoginAsync(login);
                if (userToUpdate == null)
                    return Result.Fail("Пользователь для обновления не найден");

                if (!currentUser.Admin && (currentUser.Login != login || userToUpdate.RevokedOn != null))
                    return Result.Fail("Недостаточно прав для обновления");

                userToUpdate.Name = dto.Name ?? userToUpdate.Name;
                userToUpdate.Gender = dto.Gender ?? userToUpdate.Gender;
                userToUpdate.Birthday = dto.Birthday ?? userToUpdate.Birthday;
                userToUpdate.ModifiedBy = currentUserLogin;
                userToUpdate.ModifiedOn = DateTime.UtcNow;

                await _userRepository.UpdateAsync(userToUpdate);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя {Login}", login);
                return Result.Fail("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result> ChangePasswordAsync(string login, ChangePasswordDto dto, string currentUserLogin)
        {
            try
            {
                var currentUser = await _userRepository.GetByLoginAsync(currentUserLogin);
                if (currentUser == null)
                    return Result.Fail("Пользователь не найден");

                var userToUpdate = await _userRepository.GetByLoginAsync(login);
                if (userToUpdate == null)
                    return Result.Fail("Пользователь не найден");

                if (!currentUser.Admin && (currentUser.Login != login || userToUpdate.RevokedOn != null))
                    return Result.Fail("Недостаточно прав для смены пароля");

                userToUpdate.Password = _passwordHasher.Hash(dto.NewPassword);
                userToUpdate.ModifiedBy = currentUserLogin;
                userToUpdate.ModifiedOn = DateTime.UtcNow;

                await _userRepository.UpdateAsync(userToUpdate);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при смене пароля для пользователя {Login}", login);
                return Result.Fail("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result> ChangeLoginAsync(string oldLogin, ChangeLoginDto dto, string currentUserLogin)
        {
            try
            {
                var currentUser = await _userRepository.GetByLoginAsync(currentUserLogin);
                if (currentUser == null)
                    return Result.Fail("Пользователь не найден");

                var userToUpdate = await _userRepository.GetByLoginAsync(oldLogin);
                if (userToUpdate == null)
                    return Result.Fail("Пользователь не найден");

                if (!currentUser.Admin && (currentUser.Login != oldLogin || userToUpdate.RevokedOn != null))
                    return Result.Fail("Недостаточно прав для смены логина");

                if (await _userRepository.IsLoginExistsAsync(dto.NewLogin))
                    return Result.Fail("Новый логин уже занят");

                userToUpdate.Login = dto.NewLogin;
                userToUpdate.ModifiedBy = currentUserLogin;
                userToUpdate.ModifiedOn = DateTime.UtcNow;

                await _userRepository.UpdateAsync(userToUpdate);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при смене логина с {OldLogin}", oldLogin);
                return Result.Fail("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result<IEnumerable<UserResponseDto>>> GetActiveUsersAsync(string adminLogin)
        {
            try
            {
                var admin = await _userRepository.GetByLoginAsync(adminLogin);
                if (admin == null || !admin.Admin || admin.RevokedOn != null)
                    return Result.Fail<IEnumerable<UserResponseDto>>("Только активные администраторы могут просматривать список");

                var users = await _userRepository.GetActiveUsersAsync();
                return Result.Ok(users.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка активных пользователей");
                return Result.Fail<IEnumerable<UserResponseDto>>("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result<UserResponseDto>> GetUserByLoginAsync(string login, string adminLogin)
        {
            try
            {
                var admin = await _userRepository.GetByLoginAsync(adminLogin);
                if (admin == null || !admin.Admin || admin.RevokedOn != null)
                    return Result.Fail<UserResponseDto>("Только активные администраторы могут просматривать пользователей");

                var user = await _userRepository.GetByLoginAsync(login);
                if (user == null)
                    return Result.Fail<UserResponseDto>("Пользователь не найден");

                return Result.Ok(MapToDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя {Login}", login);
                return Result.Fail<UserResponseDto>("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result<UserResponseDto>> GetSelfAsync(string login, string password)
        {
            try
            {
                var user = await _userRepository.GetByLoginAsync(login);
                
                if (user == null || user.RevokedOn != null || 
                    !_passwordHasher.Verify(user.Password, password))
                {
                    return Result.Fail<UserResponseDto>("Неверные учетные данные или пользователь деактивирован");
                }

                return Result.Ok(MapToDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных пользователя {Login}", login);
                return Result.Fail<UserResponseDto>("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result<IEnumerable<UserResponseDto>>> GetUsersOlderThanAsync(int age, string adminLogin)
        {
            try
            {
                var admin = await _userRepository.GetByLoginAsync(adminLogin);
                if (admin == null || !admin.Admin || admin.RevokedOn != null)
                    return Result.Fail<IEnumerable<UserResponseDto>>("Только активные администраторы могут выполнять этот запрос");

                if (age <= 0)
                    return Result.Fail<IEnumerable<UserResponseDto>>("Возраст должен быть положительным числом");

                var users = await _userRepository.GetUsersOlderThanAsync(age);
                return Result.Ok(users.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователей старше {Age}", age);
                return Result.Fail<IEnumerable<UserResponseDto>>("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result> DeleteUserAsync(string login, bool softDelete, string adminLogin)
        {
            try
            {
                var admin = await _userRepository.GetByLoginAsync(adminLogin);
                if (admin == null || !admin.Admin || admin.RevokedOn != null)
                    return Result.Fail("Только активные администраторы могут удалять пользователей");

                var userToDelete = await _userRepository.GetByLoginAsync(login);
                if (userToDelete == null)
                    return Result.Fail("Пользователь не найден");

                if (softDelete)
                {
                    await _userRepository.SoftDeleteAsync(login, adminLogin);
                }
                else
                {
                    await _userRepository.DeleteAsync(userToDelete);
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя {Login}", login);
                return Result.Fail("Внутренняя ошибка сервера");
            }
        }

        public async Task<Result> RestoreUserAsync(string login, string adminLogin)
        {
            try
            {
                var admin = await _userRepository.GetByLoginAsync(adminLogin);
                if (admin == null || !admin.Admin || admin.RevokedOn != null)
                    return Result.Fail("Только активные администраторы могут восстанавливать пользователей");

                var userToRestore = await _userRepository.GetByLoginAsync(login);
                if (userToRestore == null)
                    return Result.Fail("Пользователь не найден");

                if (userToRestore.RevokedOn == null)
                    return Result.Fail("Пользователь не был деактивирован");

                await _userRepository.RestoreAsync(login);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при восстановлении пользователя {Login}", login);
                return Result.Fail("Внутренняя ошибка сервера");
            }
        }

        private UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                Login = user.Login,
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday,
                IsActive = user.RevokedOn == null,
                CreatedOn = user.CreatedOn,
                ModifiedOn = user.ModifiedOn,
                CreatedBy = user.CreatedBy,
                Admin = user.Admin
            };
        }
    }
}