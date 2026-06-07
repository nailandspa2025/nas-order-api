using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.PaymentProviders.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.PaymentProviders.Commands.CreatePaymentProviderCommand;

public record CreatePaymentProviderCommand : IRequest<ApiResponse<PaymentProviderDto>>
{
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

public class CreatePaymentProviderCommandHandler : IRequestHandler<CreatePaymentProviderCommand, ApiResponse<PaymentProviderDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public CreatePaymentProviderCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaymentProviderDto>> Handle(CreatePaymentProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = new PaymentProvider
        {
            PaymentMethod = request.PaymentMethod,
            IsActive = request.IsActive,
            Description = request.Description,
        };
        if (request.Settings.Any())
        {
            var settings = request.Settings.Select(x => new PaymentProviderSetting
            {
                Key = x.Key,
                Value = x.Value,
                IsEncrypted = x.IsEncrypted
            }).ToList();
            entity.SetPaymentProviderSettings(settings);
        }
        _context.PaymentProvider.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<PaymentProviderDto>.Success(_mapper.Map<PaymentProviderDto>(entity));
    }
}