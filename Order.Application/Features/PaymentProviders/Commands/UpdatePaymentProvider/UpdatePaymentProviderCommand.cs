using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.PaymentProviders.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.PaymentProviders.Commands.UodatePaymentProviderCommand;

public record UpdatePaymentProviderCommand : IRequest<ApiResponse<PaymentProviderDto>>
{
    public int Id { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsActive { get; init; } 
    public List<RequestProviderSetting> Settings { get; init; } = new List<RequestProviderSetting>();
}

public class RequestProviderSetting
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsEncrypted { get; set; }
}
public class UpdatePaymentProviderCommandHandler : IRequestHandler<UpdatePaymentProviderCommand, ApiResponse<PaymentProviderDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public UpdatePaymentProviderCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaymentProviderDto>> Handle(UpdatePaymentProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PaymentProvider
        .Include(x => x.PaymentProviderSettings)
        .Where(x => x.Id == request.Id).FirstOrDefaultAsync();
        if (entity == null)
        {
            throw new NotFoundException(nameof(PaymentProvider), request.Id);
        }
        entity.PaymentMethod = request.PaymentMethod;
        entity.IsActive = request.IsActive;
        entity.Description = request.Description;
         var settings = new List<PaymentProviderSetting>();
        if (request.Settings.Any())
        {
            settings = request.Settings.Select(x => new PaymentProviderSetting
            {
                Key = x.Key,
                Value = x.Value,
                IsEncrypted = x.IsEncrypted
            }).ToList();
        }
        entity.SetPaymentProviderSettings(settings);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<PaymentProviderDto>.Success(_mapper.Map<PaymentProviderDto>(entity));
    }
}