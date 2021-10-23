using ChatTrials.DTO;
using ChatTrials.DTO.Models;

using System.Threading.Tasks;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterDTO model);

    Task<AuthenticationModel> LoginAsync(LoginDTO loginDTO);

    Task<string> AddRoleAsync(AddRoleDTO addRoleDTO);
}