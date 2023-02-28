using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
namespace Application.Features.MarkFeatures.Queries
{
    public class GetFirstMarkByValueIdQuery : IRequest<Mark>
    {
        public int ValueId { get; set; }
        public class GetFirstMarkByValueIdQueryHandler
            : IRequestHandler<GetFirstMarkByValueIdQuery, Mark>
        {
            private readonly IApplicationDbContext _context;
            public GetFirstMarkByValueIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Mark> Handle
                (GetFirstMarkByValueIdQuery query, CancellationToken cancellationToken)
            {
                var mark = await _context.Marks
                    .FirstOrDefaultAsync(m => m.ValueId == query.ValueId);
                return mark;
            }
        }
    }
}
