using ProductDemo.ViewModels;

public interface IUserService
{
    Task<IEnumerable<UserViewModel>> GetAllUsersAsync();
    Task<UserViewModel> GetUserByIdAsync(string id);
    Task<bool> CreateUserAsync(UserViewModel user);
    Task<bool> UpdateUserAsync(UserViewModel user);
    Task<bool> DeleteUserAsync(string id);
    Task<bool> ResetPasswordAsync(string id, string newPassword);
}
