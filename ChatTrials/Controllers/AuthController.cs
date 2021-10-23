using ChatTrials.DTO;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatTrials.Controllers
{
    //[Authorize(Roles = Constants.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var res = await _userService.RegisterAsync(registerDTO);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var authenticationModel = await _userService.LoginAsync(loginDTO);
            return Ok(authenticationModel);
        }

        [HttpPost("role")]
        public async Task<IActionResult> AddNewRole(AddRoleDTO addRoleDTO)
        {
            var res = await _userService.AddRoleAsync(addRoleDTO);
            return Ok(res);
        }
    }
}