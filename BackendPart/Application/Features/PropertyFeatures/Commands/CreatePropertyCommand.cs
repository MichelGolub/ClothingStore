using MediatR;
using Application.Interfaces;
using Domain.Entities;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.PropertyFeatures.Commands
{
    public class CreatePropertyCommand : IRequest<int>
    {
        public string Name { get; set; }
        public int TypeId { get; set; }
        public int ShopId { get; set; }

        public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreatePropertyCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreatePropertyCommand command, CancellationToken cancellationToken)
            {
                var property = new Property();
                property.Name = command.Name;
                property.TypeId = command.TypeId;
                property.ShopId = command.ShopId;

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();
                return property.Id;
            }
        }
    }
}
