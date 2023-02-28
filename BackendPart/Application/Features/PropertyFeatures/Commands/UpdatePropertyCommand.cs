using MediatR;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.PropertyFeatures.Commands
{
    public class UpdatePropertyCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public int ShopId { get; set; }

        public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, int>
        {
            private readonly IApplicationDbContext _context;

            public UpdatePropertyCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(UpdatePropertyCommand command, CancellationToken cancellationToken)
            {
                var property = await _context.Properties.FindAsync(command.Id);

                if (property == null)
                {
                    return default;
                }
                else
                {
                    property.Name = command.Name;
                    property.TypeId = command.TypeId;
                    property.ShopId = command.ShopId;
                    await _context.SaveChangesAsync();
                    return property.Id;
                }
            }
        }
    }
}
