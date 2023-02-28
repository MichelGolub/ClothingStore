using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ConnectionFeatures.Queries
{
    public class GetConnectionByUserIdQuery : IRequest<UserConnection>
    {
        public string UserId { get; set; }
        public class GetConnectionByUserIdQueryHandler : IRequestHandler<GetConnectionByUserIdQuery, UserConnection>
        {
            private readonly IApplicationDbContext _context;

            public GetConnectionByUserIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<UserConnection> Handle
                (GetConnectionByUserIdQuery query, CancellationToken cancellationToken)
            {
                var connection = await _context.Connections
                    .FirstOrDefaultAsync(c => c.UserId == query.UserId);
                return connection;
            }
        }
    }
}
