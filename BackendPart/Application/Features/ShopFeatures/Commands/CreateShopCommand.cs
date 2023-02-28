using MediatR;
using Domain.Entities;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.ShopFeatures.Commands
{
    public class CreateShopCommand : IRequest<int>
    {
        public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreateShopCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreateShopCommand command, CancellationToken cancellationToken)
            {
                var shop = new Shop();
                _context.Shops.Add(shop);
                await _context.SaveChangesAsync();
                return shop.Id;
            }
        }
    }
}
