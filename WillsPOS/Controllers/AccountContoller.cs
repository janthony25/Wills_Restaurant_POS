using BlogReact.Models;
using BlogReact.Models.Dto;
using BlogReact.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace BlogReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountContoller : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountContoller(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenService tokenService,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user == null)
                return Unauthorized(new AuthResult { Success = false, Errors = new List<string> { "Invalid email or password" } });
            

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new AuthResult { Success = false, Errors = new List<string> { "Invalid email or password" } });

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create token
            var token = _tokenService.CreateToken(user, roles);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList(),
                Token = token
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(new AuthResult { Success = false, Errors = new List<string> { "Email already exists." } });

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new AuthResult { Success = false, Errors = errors });
            }

            // Ensure roles exists
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            // Assign default role to the user
            await _userManager.AddToRoleAsync(user, "User");

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create token
            var token = _tokenService.CreateToken(user, roles);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList(),
                Token = token
            };

        }

        [Authorize]
        [HttpGet("current-user")] 
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            if (user == null)
                return NotFound();

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create token
            var token = _tokenService.CreateToken(user, roles);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList(),
                Token = token
            };
        }
    }
}
