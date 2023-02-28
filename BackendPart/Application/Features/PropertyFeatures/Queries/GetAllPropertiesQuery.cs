using MediatR;
using Application.Interfaces;
using Domain.Entities;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.PropertyFeatures.Queries
{
    public class GetAllPropertiesQuery : IRequest<IEnumerable<Property>>
    {
        public class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, IEnumerable<Property>>
        {
            private readonly IApplicationDbContext _context;
            public GetAllPropertiesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Property>> Handle
                (GetAllPropertiesQuery query, CancellationToken cancellationToken)
            {
                var properties = await _context.Properties.ToListAsync();

                if (properties == null)
                {
                    properties = new List<Property>();
                }

                return properties.AsReadOnly();
            }
        }
    }
}
