using Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Application.Features.ConnectionFeatures.Queries;
using System.Collections.Generic;
using Application.Features.ProductFeatures.Queries;

namespace WebApi.Hubs
{
    public class ProductsHub : Hub
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= Context.GetHttpContext().
            RequestServices.GetService<IMediator>();

        //[Authorize]
        public async Task Send(int[] products)
        {
            List<int> correctProducts = new List<int>();
            var shopId = 0;
            if (products.Length > 0) 
            {
                var product = await Mediator.Send(new GetProductByIdQuery() { Id = products[0] });
                if (product != null)
                {
                    shopId = (int)product.ShopId;
                    correctProducts.Add(products[0]);
                }
                else
                {
                    return;
                }
            }

            for (var i = 1; i < products.Length; i++)
            {
                var product = await Mediator.Send(new GetProductByIdQuery() { Id = products[i] });
                if (product?.ShopId == shopId)
                {
                    correctProducts.Add(products[i]);
                }
            }

            if (correctProducts.Count == 0)
            {
                return;
            }

            var userConnections = await Mediator
                .Send(new GetConnectionsByShopIdQuery { ShopId = shopId });
            if (userConnections != null) {
                List<string> usersId = new List<string>();
                foreach (var connection in userConnections)
                {
                    usersId.Add(connection.UserId);
                }
                await this.Clients.Users(usersId).SendAsync("Recieve", correctProducts.ToArray());
            }
        }
    }
}
