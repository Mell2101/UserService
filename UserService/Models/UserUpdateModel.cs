using DBConnection.Enum;

namespace UserService.Models;

public class UserUpdateModel
{
    public string? Name { get; set; }
    public UserGenderEnum? Gender { get; set; }
    public DateTime? Birthday { get; set; }
}
