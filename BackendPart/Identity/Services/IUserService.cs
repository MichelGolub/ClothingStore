using Identity.Entities;
using Identity.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.Services
{
    public interface IUserService
    {
        Task<OperationResult> RegisterAsync(RegisterModel model);
        Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model);
        Task<AuthenticationModel> RefreshTokenAsync(string token);
        Task<OperationResult> AddRoleAsync(AddRoleModel model);
        Task<User> GetById(string id);
        Task<User> GetByEmail(string email);
        bool RevokeToken(string token);
        string GetUserId(ClaimsPrincipal user);
    }
}
