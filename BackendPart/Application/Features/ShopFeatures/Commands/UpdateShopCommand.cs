using MediatR;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.ShopFeatures.Commands
{
    public class UpdateShopCommand : IRequest<int>
    {
        public int Id { get; set; }
        public class UpdateShopCommandHandler : IRequestHandler<UpdateShopCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public UpdateShopCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle
                (UpdateShopCommand command, CancellationToken cancellationToken)
            {
                var shop = await _context.Shops.FindAsync(command.Id);

                if (shop == null)
                {
                    return default;
                }
                else
                {
                    await _context.SaveChangesAsync();
                    return shop.Id;
                }
            }
        }
    }
}
