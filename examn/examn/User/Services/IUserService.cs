using examn.User.Dto;

namespace examn.User.Services;

public interface IUserService
{
    public Task<IEnumerable<UserResponse>> GetAllAsync();
    public Task<UserResponse?> GetByIdAsync(long guid);
    
    public Task<UserResponse?> GetByUsernameAsync(string username);
    
    public Task<UserResponse> CreateAsync(UserRequest userRequest);
    
    public Task<UserResponse?> UpdateAsync(long guid, UserRequestUpdate userRequestUpdate);
    
    public Task<UserResponse?> DeleteByGuidAsync(long guid);
    
    string Authenticate(string username, string password);
    
    Models.User? GetAuthenticatedUser();
}