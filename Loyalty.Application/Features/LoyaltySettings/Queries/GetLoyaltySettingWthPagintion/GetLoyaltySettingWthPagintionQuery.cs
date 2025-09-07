using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using Loyalty.Application.Common.Interfaces;
using Loyalty.Application.Features.LoyaltySettings.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Loyalty.Application.Features.LoyaltySettings.Queries.GetLoyaltySettingWthPagintion;

public record GetLoyaltySettingWthPagintionQuery: IRequest<ApiResponse<PaginatedList<LoyaltySettingDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }

    public int ? MerchantId { get; init; }
}

public class GetLoyaltySettingWthPagintionQueryHandler : IRequestHandler<GetLoyaltySettingWthPagintionQuery, ApiResponse<PaginatedList<LoyaltySettingDto>>>
{
    private readonly ILoyaltyDbContext _context;
    private readonly IMapper _mapper;

    public GetLoyaltySettingWthPagintionQueryHandler (ILoyaltyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<LoyaltySettingDto>>> Handle(GetLoyaltySettingWthPagintionQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.LoyaltySetting.Where(x =>! x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => s.Name.ToLower().Contains(paramSearchText.ToLower()));
        }
        if (request.MerchantId.HasValue)
        {
            query = query.Where(x => x.MerchantId == request.MerchantId.Value);
        }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<LoyaltySettingDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<LoyaltySettingDto>>.Success(paginationResult);
    }
}
