using System.ComponentModel.DataAnnotations;
using DBConnection.Enum;

namespace UserService.Models;

public class UserCreateModel
{
    [Required] public string Login { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Name { get; set; }
    [Required] public UserGenderEnum Gender { get; set; }
    public DateTime? Birthday { get; set; }
    [Required] public bool IsAdmin { get; set; }
}
