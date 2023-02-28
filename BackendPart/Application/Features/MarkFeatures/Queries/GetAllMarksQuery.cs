using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MarkFeatures.Queries
{
    public class GetAllMarksQuery : IRequest<IEnumerable<Mark>>
    {
        public class GetAllMarksQueryHandler : IRequestHandler<GetAllMarksQuery, IEnumerable<Mark>>
        {
            private readonly IApplicationDbContext _context;
            public GetAllMarksQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<IEnumerable<Mark>> Handle(GetAllMarksQuery query, CancellationToken cancellationToken)
            {
                var marks = await _context.Marks.ToListAsync();
                if (marks == null)
                {
                    marks = new List<Mark>();
                }
                return marks.AsReadOnly();
            }
        }
    }
}
