using Application.Features.ConnectionFeatures.Commands;
using Application.Features.ConnectionFeatures.Queries;
using Application.Features.ShopFeatures.Commands;
using Identity.Constants;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Controllers.Base;

namespace WebApi.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetUsers()
        {
            var userId = _userService.GetUserId(User);
            var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
            var connections = await Mediator
                .Send(new GetConnectionsByShopIdQuery { ShopId = userConnection.ShopId });

            List<User> users = new List<User>();
            foreach (var connection in connections)
            {
                var user = await _userService.GetById(connection.UserId);
                users.Add(user);
            }

            users.Remove(await _userService.GetById(userId));
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            var result = await _userService.RegisterAsync(model);

            if (result.Status == Identity.Entities.OperationResult.Statuses.Ok)
            {
                var user = await _userService.GetByEmail(model.Email);

                if (User.Identity.IsAuthenticated)
                {
                    var authUserId = _userService.GetUserId(User);
                    var authUserConnection = await Mediator
                        .Send(new GetConnectionByUserIdQuery { UserId = authUserId });
                    await Mediator.Send
                    (
                        new CreateConnectionCommand
                        {
                            ShopId = authUserConnection.ShopId,
                            UserId = user.Id
                        }
                    );
                }
                else
                {
                    await _userService.AddRoleAsync
                    (
                        new AddRoleModel
                        {
                            Email = model.Email,
                            Password = model.Password,
                            Role = Authorization.Roles.Manager.ToString()
                        }
                    );

                    var shopId = await Mediator.Send(new CreateShopCommand());
                    await Mediator.Send
                    (
                        new CreateConnectionCommand
                        {
                            ShopId = shopId,
                            UserId = user.Id
                        }
                    );
                }
            }

            return Ok(result);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync(TokenRequestModel model)
        {
            var result = await _userService.GetTokenAsync(model);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Headers["refreshToken"];
            var response = await _userService.RefreshTokenAsync(refreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync(AddRoleModel model)
        {
            var result = await _userService.AddRoleAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("tokens/id")]
        public async Task<IActionResult> GetRefreshTokens(string id)
        {
            var user = await _userService.GetById(id);
            return Ok(user.RefreshTokens);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken
            ([FromBody] RevokeTokenRequest model)
        {
            var token = model.Token ?? Request.Headers["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var response = _userService.RevokeToken(token);

            if (!response)
            {
                return NotFound(new { message = "Token not found" });
            }

            return Ok(new { message = "Token revoked" });
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddWorker(RegisterModel model)
        {
            var result = await _userService.RegisterAsync(model);

            if (result.Status == Identity.Entities.OperationResult.Statuses.Ok)
            {
                var user = await _userService.GetByEmail(model.Email);

                var managerUserId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = managerUserId });
                var shopId = userConnection.ShopId;

                await Mediator.Send
                (
                    new CreateConnectionCommand
                    {
                        ShopId = shopId,
                        UserId = user.Id
                    }
                );
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return Ok(user);
            }

            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var authUserConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = user.Id });
                if (userConnection.ShopId != authUserConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }
            }

            return Ok(user);
        }
    }
}
