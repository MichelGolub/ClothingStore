using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MarkFeatures.Queries
{
    public class GetMarkByIdQuery : IRequest<Mark>
    {
        public int Id { get; set; }
        public class GetMarkByIdQueryHandler : IRequestHandler<GetMarkByIdQuery, Mark>
        {
            private readonly IApplicationDbContext _context;
            public GetMarkByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Mark> Handle(GetMarkByIdQuery query, CancellationToken cancellationToken)
            {
                var mark = await _context.Marks.FindAsync(query.Id);
                return mark;
            }
        }
    }
}
