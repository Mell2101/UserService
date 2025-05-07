namespace UserService.Services;

using BCrypt.Net;

public class PasswordHasherImpl : IPasswordHasher
{
    public string Hash(string password) => BCrypt.HashPassword(password);
    public bool Verify(string passwordHash, string inputPassword) => 
        BCrypt.Verify(inputPassword, passwordHash);
}
