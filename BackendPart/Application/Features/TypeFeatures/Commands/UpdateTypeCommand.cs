using MediatR;
using Type = Domain.Entities.Type;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.TypeFeatures.Commands
{
    public class UpdateTypeCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public class UpdateTypeCommandHandler : IRequestHandler<UpdateTypeCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public UpdateTypeCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle
                (UpdateTypeCommand command, CancellationToken cancellationToken)
            {
                var type = await _context.Types.FindAsync(command.Id);

                if (type == null)
                {
                    return default;
                }

                type.Name = command.Name;

                await _context.SaveChangesAsync();
                return type.Id;
            }
        }
    }
}
