using ChatTrials.DTO;
using ChatTrials.DTO.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatTrials.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

        public async Task<AuthenticationModel> LoginAsync(LoginDTO loginDTO)
        {
            var authenticationModel = new AuthenticationModel();
            var exisitngUser = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (exisitngUser == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"No Accounts Registered with {loginDTO.Email}.";
                return authenticationModel;
            }
            if (await _userManager.CheckPasswordAsync(exisitngUser, loginDTO.Password))
            {
                authenticationModel.IsAuthenticated = true;
                JwtSecurityToken jwtSecurityToken = await CreateJWTToken(exisitngUser);
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.Email = exisitngUser.Email;
                authenticationModel.UserName = exisitngUser.UserName;
                var rolesList = await _userManager.GetRolesAsync(exisitngUser).ConfigureAwait(false);
                authenticationModel.Roles = rolesList.ToList();
                return authenticationModel;
            }
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Incorrect email or password";
            return authenticationModel;
        }

        private async Task<JwtSecurityToken> CreateJWTToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roelsClaim = new List<Claim>();
            foreach (var item in roles)
            {
                roelsClaim.Add(new Claim("roles", item));
            }
            var claims = new[]
         {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roelsClaim);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        public async Task<string> RegisterAsync(RegisterDTO model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username
            };
            var userWithExisitngEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithExisitngEmail == null)
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Authorization.default_role.ToString());
                    return $"User Registered with username {user.UserName}";
                }
                return "Something went wrong";
            }
            return $"Email {user.Email } is already registered.";
        }

        public async Task<string> AddRoleAsync(AddRoleDTO addRoleDTO)
        {
            var user = await _userManager.FindByEmailAsync(addRoleDTO.Email);
            if (user == null)
            {
                return $"No Accounts Registered with {addRoleDTO.Email}.";
            }
            if (await _userManager.CheckPasswordAsync(user, addRoleDTO.Password))
            {
                var roleExists = Enum.GetNames(typeof(Roles)).Any(x => x.ToLower() == addRoleDTO.Role.ToLower());
                if (roleExists)
                {
                    var validRole = Enum.GetValues(typeof(Roles)).Cast<Roles>().Where(x => x.ToString().ToLower() == addRoleDTO.Role.ToLower()).FirstOrDefault();
                    await _userManager.AddToRoleAsync(user, validRole.ToString());
                    return $"Added {addRoleDTO.Role} to user {addRoleDTO.Email}.";
                }
                return $"Role {addRoleDTO.Role} not found.";
            }
            return $"Incorrect Credentials for user {user.Email}.";
        }
    }
}