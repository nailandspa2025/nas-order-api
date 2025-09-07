using AutoMapper;
using BuildingBlocks.Core.Response;
using Loyalty.Application.Common.Interfaces;
using Loyalty.Application.Features.LoyaltySettings.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Loyalty.Application.Features.LoyaltySettings.Queries.GetLoyaltySettings;

public record GetLoyaltySettingByIdsQuery: IRequest<ApiResponse<IEnumerable<LoyaltySettingDto>>>
{
    public string Ids { get; init; } = null!;
}

public class GetLoyaltySettingByIdsQueryHandler : IRequestHandler<GetLoyaltySettingByIdsQuery, ApiResponse<IEnumerable<LoyaltySettingDto>>>
{
    private readonly ILoyaltyDbContext _context;
    private readonly IMapper _mapper;

    public GetLoyaltySettingByIdsQueryHandler(ILoyaltyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<LoyaltySettingDto>>> Handle(GetLoyaltySettingByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");
        var entities = await _context.LoyaltySetting
           .AsNoTracking()
           .Where(x => ids.Contains(x.Id.ToString()))
           .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<LoyaltySettingDto>>.Success(_mapper.Map<IEnumerable<LoyaltySettingDto>>(entities));
        
    }
}
