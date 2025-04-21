using BlogReact.Models;
using BlogReact.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogReact.Controllers
{
    [Authorize(Roles =  "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("users")] 
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return userDtos;
        }

        [HttpGet("dashboard")]
        public ActionResult<object> GetAdminDashboard()
        {
            return new
            {
                Message = "Welcome to the Admin Dashboard",
                AccessLevel = "Admin"
            };
        }
    }
}
