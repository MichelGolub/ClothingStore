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
    public class GetMarksByValueIdQuery : IRequest<IEnumerable<Mark>>
    {
        public int ValueId { get; set; }
        public class GetMarksByValueIdQueryHandler 
            : IRequestHandler<GetMarksByValueIdQuery, IEnumerable<Mark>>
        {
            private readonly IApplicationDbContext _context;
            public GetMarksByValueIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Mark>> Handle(GetMarksByValueIdQuery query, CancellationToken cancellationToken)
            {
                var marks = await _context.Marks.ToListAsync();
                marks = marks.Where(m => m.ValueId == query.ValueId).ToList();
                return marks;
            }
        }
    }
}
