using Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ConnectionFeatures.Commands
{
    public class DeleteConnectionByIdCommand : IRequest<int>
    {
        public int ConnectionId { get; set; }

        public class DeleteConnectionByIdCommandHandler : IRequestHandler<DeleteConnectionByIdCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public DeleteConnectionByIdCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(DeleteConnectionByIdCommand command, CancellationToken cancellatioToken)
            {
                var connection = await _context.Connections
                    .FindAsync(command.ConnectionId);

                if (connection == null)
                {
                    return default;
                }

                _context.Connections.Remove(connection);
                await _context.SaveChangesAsync();
                return connection.Id;
            }
        }
    }
}
