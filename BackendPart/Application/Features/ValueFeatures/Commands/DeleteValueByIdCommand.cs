using MediatR;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.ValueFeatures.Commands
{
    public class DeleteValueByIdCommand : IRequest<int>
    {
        public int Id { get; set; }
        public class DeleteValueByIdCommandHandler : IRequestHandler<DeleteValueByIdCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public DeleteValueByIdCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle
                (DeleteValueByIdCommand command, CancellationToken cancellationToken)
            {
                var value = await _context.Values.FindAsync(command.Id);
                if (value == null)
                {
                    return default;
                }

                _context.Values.Remove(value);
                await _context.SaveChangesAsync();
                return value.Id;
            }
        }
    }
}