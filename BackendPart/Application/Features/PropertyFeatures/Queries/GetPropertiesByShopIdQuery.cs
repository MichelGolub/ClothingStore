using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PropertyFeatures.Queries
{
    public class GetPropertiesByShopIdQuery : IRequest<IEnumerable<Property>>
    {
        public int ShopId { get; set; }
        public class GetPropertiesByShopIdQueryHandler : IRequestHandler<GetPropertiesByShopIdQuery, IEnumerable<Property>>
        {
            private readonly IApplicationDbContext _context;

            public GetPropertiesByShopIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Property>> Handle
                (GetPropertiesByShopIdQuery query, CancellationToken cancellationToken)
            {
                var properties = await _context.Properties.ToListAsync();
                properties = properties.Where(p => p.ShopId == query.ShopId).ToList();
                return properties;
            }
        }
    }
}
