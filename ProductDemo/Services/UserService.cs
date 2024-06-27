using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Models;
using ProductDemo.ViewModels;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
    {
        return await _userManager.Users.Select(u => new UserViewModel
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName
        }).ToListAsync();
    }

    public async Task<UserViewModel> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return null!;
        return new UserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    public async Task<bool> CreateUserAsync(UserViewModel user)
    {
        var applicationUser = new ApplicationUser
        {
            UserName = user.Email,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        var result = await _userManager.CreateAsync(applicationUser, user.Password!);
        return result.Succeeded;
    }

    public async Task<bool> UpdateUserAsync(UserViewModel user)
    {
        var applicationUser = await _userManager.FindByIdAsync(user.Id!);
        if (applicationUser == null) return false;
        applicationUser.Email = user.Email;
        applicationUser.UserName = user.Email;
        applicationUser.FirstName = user.FirstName;
        applicationUser.LastName = user.LastName;
        var result = await _userManager.UpdateAsync(applicationUser);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ResetPasswordAsync(string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }
}
