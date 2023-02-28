using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ConnectionFeatures.Commands
{
    public class CreateConnectionCommand : IRequest<int>
    {
        public int ShopId { get; set; }
        public string UserId { get; set; }

        public class CreateConnectionCommandHandler : IRequestHandler<CreateConnectionCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreateConnectionCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreateConnectionCommand command, CancellationToken cancellatioToken)
            {
                var connection = new UserConnection();
                connection.UserId = command.UserId;
                connection.ShopId = command.ShopId;
                _context.Connections.Add(connection);
                await _context.SaveChangesAsync();
                return connection.Id;
            }
        }
    }
}
