using Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MarkFeatures.Commands
{
    public class UpdateMarkCommand :IRequest<int>
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ValueId { get; set; }

        public class UpdateMarkCommandHandler : IRequestHandler<UpdateMarkCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public UpdateMarkCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(UpdateMarkCommand command, CancellationToken cancellationToken)
            {
                var mark = await _context.Marks.FindAsync(command.Id);

                if (mark == null)
                {
                    return default;
                }
                else
                {
                    mark.ProductId = command.ProductId;
                    mark.ValueId = command.ValueId;
                    await _context.SaveChangesAsync();
                    return mark.Id;
                }
            }
        }
    }
}
