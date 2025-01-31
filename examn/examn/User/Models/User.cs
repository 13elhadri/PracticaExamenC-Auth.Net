namespace examn.User.Models;

public class User
{
    public long Id { get; set; }
    
    public  string Username { get; set; }
    
    public  string Password { get; set; }

    public Role Role { get; set; } = Role.User;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}