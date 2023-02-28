using MediatR;
using Type = Domain.Entities.Type;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.TypeFeatures.Commands
{
    public class DeleteTypeByIdCommand : IRequest<int>
    {
        public int Id { get; set; }
        public class DeleteTypeByIdCommandHandler : IRequestHandler<DeleteTypeByIdCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public DeleteTypeByIdCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle
                (DeleteTypeByIdCommand command, CancellationToken cancellationToken)
            {
                var type = await _context.Types.FindAsync(command.Id);
                if (type == null)
                {
                    return default;
                }
                else
                {
                    _context.Types.Remove(type);
                    await _context.SaveChangesAsync();
                    return type.Id;
                }
            }
        }
    }
}
