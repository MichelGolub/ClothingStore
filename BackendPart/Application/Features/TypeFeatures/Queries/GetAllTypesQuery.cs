using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Type = Domain.Entities.Type;

namespace Application.Features.TypeFeatures.Queries
{
    public class GetAllTypesQuery : IRequest<IEnumerable<Type>>
    {
        public int Id { get; set; }
        public class GetAllTypesQueryHandler : IRequestHandler<GetAllTypesQuery, IEnumerable<Type>>
        {
            private readonly IApplicationDbContext _context;
            public GetAllTypesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Type>> Handle
                (GetAllTypesQuery query, CancellationToken cancellationToken)
            {
                var types = await _context.Types.ToListAsync();
                if (types == null)
                {
                    types = new List<Type>();
                }
                return types;
            }
        }
    }
}
