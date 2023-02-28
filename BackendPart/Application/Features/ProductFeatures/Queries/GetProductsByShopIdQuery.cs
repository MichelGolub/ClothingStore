using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ProductFeatures.Queries
{
    public class GetProductsByShopIdQuery : IRequest<IEnumerable<Product>>
    {
        public int ShopId { get; set; }
        public class GetProductsByShopIdQueryHander : IRequestHandler<GetProductsByShopIdQuery, IEnumerable<Product>>
        {
            private readonly IApplicationDbContext _context;
            public GetProductsByShopIdQueryHander(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<IEnumerable<Product>> Handle
                (GetProductsByShopIdQuery query, CancellationToken cancellationToken)
            {
                var products = await _context.Products.ToListAsync();
                products = products.Where(p => p.ShopId == query.ShopId).ToList();
                return products;
            }
        }
    }
}
