using System.ComponentModel.DataAnnotations;
using DBConnection.Enum;

namespace UserService.Dto;

    public class UserCreateDto
    {
        [property: Required(ErrorMessage = "Логин обязателен")]
        [property: RegularExpression(@"^[a-zA-Z0-9]{3,50}$", ErrorMessage = "Только латинские буквы и цифры")]
        public string Login {get; set;}

        [property: Required(ErrorMessage = "Пароль обязателен")]
        [property: RegularExpression(@"^[a-zA-Z0-9]{6,100}$", ErrorMessage = "Только латинские буквы и цифры")]
        public string Password {get; set;}

        [property: Required(ErrorMessage = "Имя обязательно")]
        [property: RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Только буквы")]
        public string Name {get; set;}

        [property: Range(0, 2, ErrorMessage = "Пол: 0-жен, 1-муж, 2-неизвестно")]
        public UserGenderEnum Gender {get; set;}

        [property: CustomValidation(typeof(PastDateValidator), nameof(PastDateValidator.Validate))]
        public DateTime? Birthday {get; set;}
        public bool IsAdmin {get; set;}
    }

    public record UserUpdateDto
    {
        [property: Required(ErrorMessage = "Имя обязательно")]
        [property: RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Только буквы")]
        public string? Name {get; set;}

        [property: Range(0, 2, ErrorMessage = "Пол: 0-жен, 1-муж, 2-неизвестно")]
        public UserGenderEnum? Gender {get; set;}

        [property: CustomValidation(typeof(PastDateValidator), nameof(PastDateValidator.Validate))]
        public DateTime? Birthday {get; set;}
    }

    public class ChangePasswordDto
    {
        [property: RegularExpression(@"^[a-zA-Z0-9]{6,100}$",
            ErrorMessage = "Пароль: только латинские буквы и цифры (6-100 символов)")]
        [property: Required(ErrorMessage = "Новый пароль обязателен")]
        public string NewPassword {get; set;}
    }

    public class ChangeLoginDto
    {
        [property: RegularExpression(@"^[a-zA-Z0-9]{3,50}$",
            ErrorMessage = "Логин: только латинские буквы и цифры (3-50 символов)")]
        [property: Required(ErrorMessage = "Новый логин обязателен")]
        public string NewLogin {get; set;}
    }

    public record UserResponseDto
    {
        public string Login { get; init; }
        public string Name { get; init; }
        public UserGenderEnum Gender { get; init; }
        public DateTime? Birthday { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedOn { get; init; }
        public DateTime ModifiedOn { get; init; }
        public string CreatedBy { get; init; }
        public bool Admin { get; init; }
    }

    public static class PastDateValidator
    {
        public static ValidationResult Validate(DateTime? date)
        {
            return date == null || date < DateTime.Now 
                ? ValidationResult.Success 
                : new ValidationResult("Дата должна быть в прошлом");
        }
    }
