using Cafe.POS.Models.Base;
using Cafe.POS.Models.Enums;

namespace Cafe.POS.Models;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public Role Role { get; set; }
}
