using Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ConnectionFeatures.Commands
{
    public class UpdateConnectionCommand : IRequest<int>
    {
        public int ConnectionId { get; set; }
        public int ShopId { get; set; }
        public string UserId { get; set; }

        public class UpdateConnectionCommandHandler : IRequestHandler<UpdateConnectionCommand, int>
        {
            private readonly IApplicationDbContext _context;

            public UpdateConnectionCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle
                (UpdateConnectionCommand command, CancellationToken cancellationToken)
            {
                var connection = await _context
                    .Connections.FindAsync(command.ConnectionId);

                if (connection == null)
                {
                    return default;
                }
                else
                {
                    connection.ShopId = command.ShopId;
                    connection.UserId = command.UserId;
                    _context.Connections.Update(connection);
                    await _context.SaveChangesAsync();
                    return connection.Id;
                }
            }
        }
    }
}
