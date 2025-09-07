using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Loyalty.Application.Common.Interfaces;
using Loyalty.Application.Features.LoyaltySettings.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Loyalty.Application.Features.LoyaltySettings.Queries.GetLoyaltySetting;

public record GetLoyaltySettingByIdQuery : IRequest<ApiResponse<LoyaltySettingDto>>
{
    public int Id { get; init; }
}

public class GetLoyaltySettingByIdQueryHandler : IRequestHandler<GetLoyaltySettingByIdQuery, ApiResponse<LoyaltySettingDto>>
{
    private readonly ILoyaltyDbContext _context;
    private readonly IMapper _mapper;

    public GetLoyaltySettingByIdQueryHandler(ILoyaltyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<LoyaltySettingDto>> Handle(GetLoyaltySettingByIdQuery request, CancellationToken cancellationToken)
    {

        var entity = await _context.LoyaltySetting
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(LoyaltySettingDto), request.Id);
        }

        return ApiResponse<LoyaltySettingDto>.Success(_mapper.Map<LoyaltySettingDto>(entity));
    }
}
