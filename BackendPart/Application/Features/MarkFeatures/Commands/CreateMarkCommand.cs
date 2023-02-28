using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Value = Domain.Entities.Value;

namespace Application.Features.MarkFeatures.Commands
{
    public class CreateMarkCommand : IRequest<int>
    {
        public int ProductId { get; set; }
        public int ValueId { get; set; }
        public Value Value { get; set; }

        public class CreateMarkCommandHandler : IRequestHandler<CreateMarkCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreateMarkCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<int> Handle(CreateMarkCommand command, CancellationToken cancellatioToken)
            {
                var mark = new Mark();
                mark.ProductId = command.ProductId;
                mark.ValueId = command.ValueId;

                if (command.Value != null)
                {
                    _context.Values.Add(command.Value);
                    mark.Value = command.Value;
                }

                _context.Marks.Add(mark);
                await _context.SaveChangesAsync();
                return mark.Id;
            }
        }
    }
}
