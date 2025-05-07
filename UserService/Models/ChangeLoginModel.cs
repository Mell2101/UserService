using System.ComponentModel.DataAnnotations;

namespace UserService.Models;

public class ChangeLoginModel
{
    [Required] public string NewLogin { get; set; }
}
