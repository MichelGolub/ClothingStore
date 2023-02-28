using MediatR;
using Domain.Entities;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ShopFeatures.Queries
{
    public class GetAllShopsQuery : IRequest<IEnumerable<Shop>>
    {
        public int Id { get; set; }
        public class GetAllShopsQueryHandler : IRequestHandler<GetAllShopsQuery, IEnumerable<Shop>>
        {
            private readonly IApplicationDbContext _context;
            public GetAllShopsQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Shop>> Handle
                (GetAllShopsQuery query, CancellationToken cancellationToken)
            {
                var shops = await _context.Shops.ToListAsync();
                if (shops == null)
                {
                    shops = new List<Shop>();
                }

                return shops;
            }
        }
    }
}
