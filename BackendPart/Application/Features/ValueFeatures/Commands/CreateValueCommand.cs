using MediatR;
using Domain.Entities;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.ValueFeatures.Commands
{
    public class CreateValueCommand : IRequest<int>
    {
        public string Name { get; set; }
        public int PropertyId { get; set; }
        public class CreateValueCommandHandler : IRequestHandler<CreateValueCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreateValueCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle
                (CreateValueCommand command, CancellationToken cancellationToken)
            {
                var value = new Value();
                value.Name = command.Name;
                value.PropertyId = command.PropertyId;

                _context.Values.Add(value);
                await _context.SaveChangesAsync();
                return value.Id;
            }
        }
    }
}
