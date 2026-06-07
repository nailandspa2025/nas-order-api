using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.PaymentProviders.Commands.DeletePaymentProviderCommand;

public record DeletePaymentProviderCommand(int Id) : IRequest<ApiResponse>;

public class DeletePaymentProviderCommandHandler : IRequestHandler<DeletePaymentProviderCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;

    public DeletePaymentProviderCommandHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeletePaymentProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PaymentProvider
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(PaymentProvider), request.Id);
        }
        _context.PaymentProvider.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();

    }
}
