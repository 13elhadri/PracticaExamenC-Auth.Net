namespace examn.User.Dto;

public class UserResponse
{
    public string Username { get; set; }
    
    public string Password { get; set; }
    
    public string Role { get; set; }
    
    public string CreatedAt { get; set; }
    
    public string UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
}