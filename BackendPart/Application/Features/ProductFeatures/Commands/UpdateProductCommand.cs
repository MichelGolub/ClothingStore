using MediatR;
using Application.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Features.ProductFeatures.Commands
{
    public class UpdateProductCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ShopId { get; set; }

        public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, int>
        {
            private readonly IApplicationDbContext _context;

            public UpdateProductCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
            {
                var product = await _context.Products.FindAsync(command.Id);

                if (product == null)
                {
                    return default;
                }
                else
                {
                    product.Name = command.Name;
                    product.ShopId = command.ShopId;

                    await _context.SaveChangesAsync();
                    return product.Id;
                }
            }
        }
    }
}
