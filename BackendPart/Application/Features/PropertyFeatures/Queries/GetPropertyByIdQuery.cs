using MediatR;
using Application.Interfaces;
using Domain.Entities;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.PropertyFeatures.Queries
{
    public class GetPropertyByIdQuery : IRequest<Property>
    {
        public int Id { get; set; }
        public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, Property>
        {
            private readonly IApplicationDbContext _context;
            public GetPropertyByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Property> Handle
                (GetPropertyByIdQuery query, CancellationToken cancellationToken)
            {
                var property = await _context.Properties.FindAsync(query.Id);
                return property;
            }
        }
    }
}
