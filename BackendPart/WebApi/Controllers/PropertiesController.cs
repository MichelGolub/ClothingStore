using Application.Features.ConnectionFeatures.Queries;
using Application.Features.PropertyFeatures.Commands;
using Application.Features.PropertyFeatures.Queries;
using Application.Features.TypeFeatures.Queries;
using Application.Features.ValueFeatures.Queries;
using Domain.Entities;
using Identity.Constants;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Controllers.Base;
using WebApi.Filters;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    public class PropertiesController : BaseApiController
    {
        private readonly IUserService _userService;
        public PropertiesController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreatePropertyCommand command)
        {
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                command.ShopId = userConnection.ShopId;
            }

            return Ok(await Mediator.Send(command));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll
            ([FromQuery] PaginationFilter queryPagination)
        {
            IEnumerable<Property> properties;
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                properties = await Mediator
                    .Send(new GetPropertiesByShopIdQuery { ShopId = userConnection.ShopId });
            }
            else
            {
                properties = await Mediator.Send(new GetAllPropertiesQuery());
            }

            var validPagination = new PaginationFilter
                (queryPagination.PageNumber, queryPagination.PageSize);
            var pagedProperties = properties
                .Skip((validPagination.PageNumber - 1) * validPagination.PageSize)
                .Take(validPagination.PageSize)
                .ToList();
            var totalProperties = properties.Count();
            var pagedResponse = PaginationHelper.CreatePagedResponse<Property>
                (pagedProperties, validPagination, totalProperties);
            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var property = await Mediator.Send(new GetPropertyByIdQuery { Id = id });
            if (property != null)
            {
                var type = await Mediator.Send(new GetTypeByIdQuery { Id = property.TypeId });
                if (type.Name == Type.Types.category.ToString())
                {
                    var values = await Mediator
                        .Send(new GetValuesByPropertyIdQuery { PropertyId = property.Id });
                    property.Values = values;
                }
            }
            return Ok(property);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) 
        {
            var property = await Mediator.Send(new GetPropertyByIdQuery { Id = id });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                if (property.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }
            }

            return Ok(await Mediator.Send(new DeletePropertyByIdCommand { Id = id }));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdatePropertyCommand command)
        {
            var property = await Mediator.Send(new GetPropertyByIdQuery { Id = id });
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                if (property.ShopId != userConnection.ShopId)
                {
                    return new StatusCodeResult(403);
                }

                if (property.TypeId != command.TypeId)
                {
                    return new StatusCodeResult(403);
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
