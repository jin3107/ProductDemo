using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDemo.ViewModels;
using ProductDemo.Models;

[Authorize]
public class UserManagementController : Controller
{
    private readonly IUserService _userService;

    public UserManagementController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        return View(new AddUserViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.CreateUserAsync(new UserViewModel
            {
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName
            });

            if (result)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to add user.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DetailUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.UpdateUserAsync(new UserViewModel
            {
                Id = model.Id,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            });

            if (result)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to update user.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new DeleteUserViewModel
        {
            Id = user.Id,
            Email = user.Email
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(DeleteUserViewModel model)
    {
        var result = await _userService.DeleteUserAsync(model.Id!);
        if (!result)
        {
            return NotFound();
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ResetPasswordUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ResetPasswordUserViewModel
        {
            Id = user.Id,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPasswordUser(ResetPasswordUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View(model);
            }

            var result = await _userService.ResetPasswordAsync(model.Id!, model.NewPassword!);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

        return View(model);
    }
}
