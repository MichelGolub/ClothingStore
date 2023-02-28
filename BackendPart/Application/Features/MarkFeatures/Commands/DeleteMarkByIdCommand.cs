using Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MarkFeatures.Commands
{
    public class DeleteMarkByIdCommand : IRequest<int>
    {
        public int Id { get; set; }
        public class DeleteMarkByIdCommandHandler : IRequestHandler<DeleteMarkByIdCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public DeleteMarkByIdCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(DeleteMarkByIdCommand command, CancellationToken cancellationToken)
            {
                var mark = await _context.Marks.FindAsync(command.Id);
                if (mark == null)
                {
                    return default;
                }
                _context.Marks.Remove(mark);
                await _context.SaveChangesAsync();
                return mark.Id;
            }
        }
    }
}
