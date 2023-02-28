using MediatR;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.ValueFeatures.Commands
{
    public class UpdateValueCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PropertyId { get; set; }

        public class UpdateValueCommandHandler : IRequestHandler<UpdateValueCommand, int>
        {
            private readonly IApplicationDbContext _context;

            public UpdateValueCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(UpdateValueCommand command, CancellationToken cancellationToken)
            {
                var value = await _context.Values.FindAsync(command.Id);

                if (value == null)
                {
                    return default;
                }
                else
                {
                    value.Name = command.Name;
                    value.PropertyId = command.PropertyId;

                    await _context.SaveChangesAsync();
                    return value.Id;
                }
            }
        }
    }
}
