using System.ComponentModel.DataAnnotations;

namespace UserService.Models;

public class ChangePasswordModel
{
    [Required] public string NewPassword { get; set; }
}
