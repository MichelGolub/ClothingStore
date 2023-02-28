using MediatR;
using Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ShopFeatures.Commands
{
    public class DeleteShopByIdCommand : IRequest<int>
    {
        public int Id { get; set; }
        public class DeleteShopByIdCommandHandler : IRequestHandler<DeleteShopByIdCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public DeleteShopByIdCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle
                (DeleteShopByIdCommand command, CancellationToken cancellationToken)
            {
                var shop = await _context.Shops.FindAsync(command.Id);

                if (shop == null)
                {
                    return default;
                }

                _context.Shops.Remove(shop);
                await _context.SaveChangesAsync();
                return shop.Id;
            }
        }
    }
}
