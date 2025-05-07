using DBConnection.Entity;
using DBConnection.Enum;

namespace DBConnection.Repository;
using BCrypt.Net;

public static class DbInitialiser
{
    public static async Task Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.User.Any(u => u.Login == "Admin"))
        {
            return; 
        }

        var now = DateTime.UtcNow;
        
        var adminUser = new User
        {
            GuidId = Guid.NewGuid(),
            Login = "Admin",
            Password = HashPassword("Admin123"), 
            Name = "System Administrator",
            Gender = UserGenderEnum.Unknown,
            Birthday = null,
            Admin = true,
            CreatedOn = now,
            CreatedBy = "System",
            ModifiedOn = now,
            ModifiedBy = "System",
            RevokedOn = null, 
            RevokedBy = null
        };

        context.User.Add(adminUser);
        await context.SaveChangesAsync();
    }
    
    public static string HashPassword(string password) => BCrypt.HashPassword(password);
}
