using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MarkFeatures.Queries
{
    public class GetMarksByProductIdQuery : IRequest<IEnumerable<Mark>>
    {
        public int ProductId { get; set; }
        public class GetMarksByProductIdQueryHandler 
            : IRequestHandler<GetMarksByProductIdQuery, IEnumerable<Mark>>
        {
            private readonly IApplicationDbContext _context;
            public GetMarksByProductIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<IEnumerable<Mark>> Handle
                (GetMarksByProductIdQuery query, CancellationToken cancellationToken)
            {
                var marks = await _context.Marks.ToListAsync();
                marks = marks.Where(m => m.ProductId == query.ProductId).ToList();
                return marks;
            }
        }
    }
}
