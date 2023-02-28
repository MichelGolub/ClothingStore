using Application.Features.ConnectionFeatures.Queries;
using Application.Features.MarkFeatures.Queries;
using Application.Features.ProductFeatures.Commands;
using Application.Features.ProductFeatures.Queries;
using Application.Features.PropertyFeatures.Queries;
using Application.Features.TypeFeatures.Queries;
using Application.Features.ValueFeatures.Commands;
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
    public class ProductsController : BaseApiController
    {
        private readonly IUserService _userService;
        public ProductsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await Mediator.Send(new GetProductByIdQuery { Id = id });
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

            var marks = await Mediator
                .Send(new GetMarksByProductIdQuery { ProductId = product.Id });
            foreach (var mark in marks)
            {
                await Mediator
                    .Send(new GetValueByIdQuery { Id = mark.ValueId });
            }

            return Ok(product);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll
            ([FromQuery] PaginationFilter queryPagination,
            [FromQuery] PropertyFilter propertyFilter)
        {
            IEnumerable<Product> products;
            if (!User.IsInRole(Authorization.Roles.Administrator.ToString()))
            {
                var userId = _userService.GetUserId(User);
                var userConnection = await Mediator
                    .Send(new GetConnectionByUserIdQuery { UserId = userId });
                products = await Mediator
                    .Send(new GetProductsByShopIdQuery { ShopId = userConnection.ShopId });
            }
            else
            {
                products = await Mediator.Send(new GetAllProductsQuery());
            }

            var filteredProducts = new List<Product>();
            foreach (int valueId in propertyFilter.NominalValuesId)
            {
                var marks = await Mediator
                    .Send(new GetMarksByValueIdQuery { ValueId = valueId });
                foreach (var mark in marks)
                {
                    var product = await Mediator
                        .Send(new GetProductByIdQuery { Id = mark.ProductId });
                    if (!filteredProducts.Contains(product))
                    {
                        filteredProducts.Add(product);
                    }
                }
                products = products.Intersect(filteredProducts).ToList();
                filteredProducts.Clear();
            }

            foreach (var numericProperty in propertyFilter.NumericPropertyRanges)
            {
                var min = numericProperty.Min;
                var max = numericProperty.Max;
                if (min > max)
                {
                    min = numericProperty.Max;
                    max = numericProperty.Min;
                }

                var values = await Mediator
                    .Send(new GetValuesByPropertyIdQuery { PropertyId = numericProperty.Id });
                foreach (var value in values)
                {
                    double normalizedValue = float.Parse(value.Name.Replace('.', ','));
                    if (normalizedValue >= min && normalizedValue <= max)
                    {
                        var mark = await Mediator
                            .Send(new GetFirstMarkByValueIdQuery { ValueId = value.Id });
                        var product = await Mediator
                                .Send(new GetProductByIdQuery { Id = mark.ProductId });
                        if (!filteredProducts.Contains(product))
                        {
                            filteredProducts.Add(product);
                        }
                    }
                }
                products = products.Intersect(filteredProducts).ToList();
                filteredProducts.Clear();
            }

            var validPagination = new PaginationFilter
                (queryPagination.PageNumber, queryPagination.PageSize);
            var pagedProducts = products
                .Skip((validPagination.PageNumber - 1) * validPagination.PageSize)
                .Take(validPagination.PageSize)
                .ToList();
            var totalProducts = products.Count();
            var pagedResponse = PaginationHelper.CreatePagedResponse<Product>
                (pagedProducts, validPagination, totalProducts);
            return Ok(pagedResponse);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateProductCommand command)
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdateProductCommand command)
        {
            var product = await Mediator.Send(new GetProductByIdQuery{ Id = id });
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

            if (command.Id != id)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await Mediator.Send(new GetProductByIdQuery { Id = id });
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

            var marks = await Mediator
                .Send(new GetMarksByProductIdQuery { ProductId = product.Id });
            await Mediator.Send(new GetAllTypesQuery());
            foreach (var mark in marks)
            {
                var value = await Mediator.Send(new GetValueByIdQuery { Id = mark.ValueId });
                var property = await Mediator.Send(new GetPropertyByIdQuery { Id = value.PropertyId });
                if (property.Type.Name == Type.Types.numeric.ToString())
                {
                    await Mediator.Send(new DeleteValueByIdCommand { Id = value.Id });
                }
            }

            return Ok(await Mediator.Send(new DeleteProductByIdCommand { Id = id}));
        }
    }
}
