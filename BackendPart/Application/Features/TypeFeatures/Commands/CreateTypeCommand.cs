using MediatR;
using Type = Domain.Entities.Type;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.TypeFeatures.Commands
{
    public class CreateTypeCommand : IRequest<int>
    {
        public string Name { get; set; }
        public class CreateTypeCommandHandler : IRequestHandler<CreateTypeCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreateTypeCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreateTypeCommand command, CancellationToken cancellationToken)
            {
                var type = new Type();
                type.Name = command.Name;
                _context.Types.Add(type);
                await _context.SaveChangesAsync();
                return type.Id;
            }
        }
    }
}
