using Application.Features.ConnectionFeatures.Queries;
using Application.Features.PropertyFeatures.Queries;
using Application.Features.TypeFeatures.Queries;
using Application.Features.ValueFeatures.Commands;
using Application.Features.ValueFeatures.Queries;
using Domain.Entities;
using Identity.Constants;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApi.Controllers.Base;
using Type = Domain.Entities.Type;

namespace WebApi.Controllers
{
    public class ValuesController : BaseApiController
    {
        private readonly IUserService _userService;
        public ValuesController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateValueCommand command)
        {
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                var property = await Mediator
                    .Send(new GetPropertyByIdQuery { Id = command.PropertyId });
                if (property.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }

                var type = await Mediator.Send(new GetTypeByIdQuery { Id = property.TypeId });
                if (type.Name == Type.Types.numeric.ToString())
                {
                    double number;
                    command.Name = command.Name.Replace('.', ',');
                    if (!double.TryParse(command.Name, out number))
                    {
                        command.Name = "0";
                    }
                }
            }

            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var value = await Mediator.Send(new GetValueByIdQuery { Id = id });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                var property = await Mediator.Send(new GetPropertyByIdQuery { Id = value.PropertyId });
                if (property.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }
            }

            return Ok(await Mediator.Send(new DeleteValueByIdCommand { Id = id }));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdateValueCommand command)
        {
            var property = await Mediator.Send(new GetPropertyByIdQuery { Id = command.PropertyId });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                if (property.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }

                var type = await Mediator.Send(new GetTypeByIdQuery { Id = property.TypeId });
                if (type.Name == Type.Types.numeric.ToString()) 
                {
                    double number;
                    command.Name = command.Name.Replace('.', ',');
                    if (!double.TryParse(command.Name, out number))
                    {
                        command.Name = "0";
                    }
                }
            }

            if (command.Id != id)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(command));
        }
    }
}
