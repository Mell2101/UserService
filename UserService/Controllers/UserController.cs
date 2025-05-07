using DBConnection.Entity;
using Microsoft.AspNetCore.Mvc;
using UserService.Dto;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Создание нового пользователя (только для админов)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<Result<User>> CreateUser(
            [FromBody] UserCreateDto dto,
            [FromHeader(Name = "X-Admin-Login")] string adminLogin)
        {
            if (!ModelState.IsValid)
            {
                return new Result<User>(new User() , false, "ModelIsNotValid");
            }

            var result = await _userService.CreateUserAsync(dto, adminLogin);
            return result;
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        [HttpPut("{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result> UpdateUser(
            string login,
            [FromBody] UserUpdateDto dto,
            [FromHeader(Name = "X-Current-User-Login")] string currentUserLogin)
        {
            if (!ModelState.IsValid)
            {
                return Result.Fail("ModelIsNotValid");
            }

            var result = await _userService.UpdateUserAsync(login, dto, currentUserLogin);
            return result;
        }

        /// <summary>
        /// Смена пароля
        /// </summary>
        [HttpPut("{login}/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<Result> ChangePassword(
            string login,
            [FromBody] ChangePasswordDto dto,
            [FromHeader(Name = "X-Current-User-Login")] string currentUserLogin)
        {
            if (!ModelState.IsValid)
            {
                return Result.Fail("ModelIsNotValid");
            }

            var result = await _userService.ChangePasswordAsync(login, dto, currentUserLogin);
            return result;
        }

        /// <summary>
        /// Смена логина
        /// </summary>
        [HttpPut("{oldLogin}/change-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<Result> ChangeLogin(
            string oldLogin,
            [FromBody] ChangeLoginDto dto,
            [FromHeader(Name = "X-Current-User-Login")] string currentUserLogin)
        {

            if (!ModelState.IsValid)
            {
                return Result.Fail("ModelIsNotValid");
            }

            var result = await _userService.ChangeLoginAsync(oldLogin, dto, currentUserLogin);
            return result;
        }

        /// <summary>
        /// Получение списка активных пользователей (только для админов)
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<Result<IEnumerable<UserResponseDto>>> GetActiveUsers(
            [FromHeader(Name = "X-Admin-Login")] string adminLogin)
        {
            if (!ModelState.IsValid)
            {
                return new Result<IEnumerable<UserResponseDto>>(new List<UserResponseDto>() , false, "ModelIsNotValid");
            }
            var result = await _userService.GetActiveUsersAsync(adminLogin);
            return result;
        }

        /// <summary>
        /// Получение пользователя по логину (только для админов)
        /// </summary>
        [HttpGet("{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result<UserResponseDto>> GetUserByLogin(
            string login,
            [FromHeader(Name = "X-Admin-Login")] string adminLogin)
        {

            if (!ModelState.IsValid)
            {
                return new Result<UserResponseDto>(new UserResponseDto() , false, "ModelIsNotValid");
            }

            var result = await _userService.GetUserByLoginAsync(login, adminLogin);
            return result;
        }

        /// <summary>
        /// Получение информации о текущем пользователе
        /// </summary>
        [HttpGet("self")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<Result<UserResponseDto>> GetSelf(
            [FromHeader(Name = "X-User-Login")] string login,
            [FromHeader(Name = "X-User-Password")] string password)
        {
            if (!ModelState.IsValid)
            {
                return new Result<UserResponseDto>(new UserResponseDto() , false, "ModelIsNotValid");
            }

            var result = await _userService.GetSelfAsync(login, password);
            return result;
        }

        /// <summary>
        /// Получение пользователей старше указанного возраста (только для админов)
        /// </summary>
        [HttpGet("older-than/{age}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<Result<IEnumerable<UserResponseDto>>> GetUsersOlderThan(
            int age,
            [FromHeader(Name = "X-Admin-Login")] string adminLogin)
        {
            if (!ModelState.IsValid)
            {
                return new Result<IEnumerable<UserResponseDto>>(new List<UserResponseDto>() , false, "ModelIsNotValid");
            }

            var result = await _userService.GetUsersOlderThanAsync(age, adminLogin);
            return result;
        }

        /// <summary>
        /// Удаление пользователя (только для админов)
        /// </summary>
        [HttpDelete("{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result> DeleteUser(
            string login,
            [FromHeader(Name = "X-Admin-Login")] string adminLogin,
            [FromQuery] bool softDelete = true)
        {
            if (!ModelState.IsValid)
            {
                return Result.Fail("ModelIsNotValid");
            }

            var result = await _userService.DeleteUserAsync(login, softDelete, adminLogin);
            return result;
        }

        /// <summary>
        /// Восстановление пользователя (только для админов)
        /// </summary>
        [HttpPut("{login}/restore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Result> RestoreUser(
            string login,
            [FromHeader(Name = "X-Admin-Login")] string adminLogin)
        {
            if (!ModelState.IsValid)
            {
                return Result.Fail("ModelIsNotValid");
            }

            var result = await _userService.RestoreUserAsync(login, adminLogin);
            return result;
        }
    }
}