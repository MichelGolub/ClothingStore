using Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Type = Domain.Entities.Type;

namespace Application.Features.TypeFeatures.Queries
{
    public class GetTypeByIdQuery : IRequest<Type>
    {
        public int Id { get; set; }
        public class GetTypeByIdQueryHandler : IRequestHandler<GetTypeByIdQuery, Type>
        {
            private readonly IApplicationDbContext _context;
            public GetTypeByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Type> Handle(GetTypeByIdQuery query, CancellationToken cancellationToken)
            {
                var type = await _context.Types.FindAsync(query.Id);
                return type;
            }
        }
    }
}
