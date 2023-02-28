using MediatR;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.PropertyFeatures.Commands
{
    public class DeletePropertyByIdCommand : IRequest<int>
    {
        public int Id { get; set; }

        public class DeletePropertyByIdCommandHandler : IRequestHandler<DeletePropertyByIdCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public DeletePropertyByIdCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(DeletePropertyByIdCommand command, CancellationToken cancellationToken)
            {
                var property = await _context.Properties.FindAsync(command.Id);

                if (property == null)
                {
                    return default;
                }

                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
                return property.Id;
            }
        }
    }
}
