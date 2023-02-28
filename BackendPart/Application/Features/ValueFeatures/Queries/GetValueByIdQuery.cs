using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ValueFeatures.Queries
{
    public class GetValueByIdQuery : IRequest<Value>
    {
        public int Id { get; set; }
        public class GetValueByIdQueryHandler : IRequestHandler<GetValueByIdQuery, Value>
        {
            private readonly IApplicationDbContext _context;
            public GetValueByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Value> Handle
                (GetValueByIdQuery query, CancellationToken cancellationToken)
            {
                var value = await _context.Values.FindAsync(query.Id);
                return value;
            }
        }
    }
}
