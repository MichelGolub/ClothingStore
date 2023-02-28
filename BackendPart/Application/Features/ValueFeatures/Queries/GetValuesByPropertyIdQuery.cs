using System.Collections.Generic;
using System.Linq;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ValueFeatures.Queries
{
    public class GetValuesByPropertyIdQuery : IRequest<IEnumerable<Value>>
    {
        public int PropertyId { get; set; }

        public class GetValuesByPropertyIdQueryHandler : IRequestHandler<GetValuesByPropertyIdQuery, IEnumerable<Value>>
        {
            private readonly IApplicationDbContext _context;
            public GetValuesByPropertyIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Value>> Handle
                (GetValuesByPropertyIdQuery query, CancellationToken cancellationToken)
            {
                var values = await _context.Values.ToListAsync();
                values = values.Where(v => v.PropertyId == query.PropertyId).ToList();
                return values;
            }
        }
    }
}
