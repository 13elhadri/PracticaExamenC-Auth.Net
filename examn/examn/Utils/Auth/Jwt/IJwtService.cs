namespace examn.Utils.Auth.Jwt;

public interface IJwtService
{
    string GenerateToken(User.Models.User user);

}