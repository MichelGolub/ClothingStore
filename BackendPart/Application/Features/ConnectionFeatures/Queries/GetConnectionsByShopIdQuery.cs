using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ConnectionFeatures.Queries
{
    public class GetConnectionsByShopIdQuery : IRequest<IEnumerable<UserConnection>>
    {
        public int ShopId { get; set; }
        public class GetConnectionsByShopIdQueryHandler 
            : IRequestHandler<GetConnectionsByShopIdQuery, IEnumerable<UserConnection>>
        {
            private readonly IApplicationDbContext _context;

            public GetConnectionsByShopIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<UserConnection>> Handle
                (GetConnectionsByShopIdQuery query, CancellationToken cancellationToken)
            {
                var connections = await _context.Connections.ToListAsync();
                connections = connections.Where(c => c.ShopId == query.ShopId ).ToList();
                return connections;
            }
        }
    }
}
