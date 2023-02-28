using MediatR;
using Domain.Entities;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.ShopFeatures.Queries
{
    public class GetShopByIdQuery : IRequest<Shop>
    {
        public int Id { get; set; }
        public class GetShopByIdQueryHandler : IRequestHandler<GetShopByIdQuery, Shop>
        {
            private readonly IApplicationDbContext _context;
            public GetShopByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Shop> Handle
                (GetShopByIdQuery query, CancellationToken cancellationToken)
            {
                var shop = await _context.Shops.FindAsync(query.Id);
                return shop;
            }
        }
    }
}
