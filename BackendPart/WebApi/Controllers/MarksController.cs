using Application.Features.ConnectionFeatures.Queries;
using Application.Features.MarkFeatures.Commands;
using Application.Features.MarkFeatures.Queries;
using Application.Features.ProductFeatures.Queries;
using Domain.Entities;
using Identity.Constants;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Controllers.Base;
using WebApi.Filters;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    public class MarksController : BaseApiController
    {

        private readonly IUserService _userService;
        public MarksController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a New Mark
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create
            (CreateMarkCommand command)
        {
            var product = await Mediator.Send(new GetProductByIdQuery { Id = command.ProductId });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                if (product.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var mark = await Mediator.Send(new GetMarkByIdQuery { Id = id });

            var product = await Mediator.Send(new GetProductByIdQuery { Id = mark.ProductId });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                if (product.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }
            }

            return Ok(mark);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var mark = await Mediator.Send(new GetMarkByIdQuery { Id = id });
            var product = await Mediator.Send(new GetProductByIdQuery { Id = mark.ProductId });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                if (product.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }
            }

            var deletedMarkId = await Mediator.Send(new DeleteMarkByIdCommand { Id = id });
            return Ok(deletedMarkId);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdateMarkCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            var mark = await Mediator.Send(new GetMarkByIdQuery { Id = id });
            var product = await Mediator.Send(new GetProductByIdQuery { Id = mark.ProductId });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                if (product.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }
            }

            return Ok(await Mediator.Send(command));
        }
    }
}
