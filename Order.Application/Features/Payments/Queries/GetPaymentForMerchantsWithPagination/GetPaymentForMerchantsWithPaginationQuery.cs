using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Application.Features.Payments.Models;

namespace Order.Application.Features.Payments.Queries.GetPaymentForMerchantsWithPagination;

public record GetPaymentForMerchantsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<PaymentDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
}

public class GetPaymentForMerchantsWithPaginationQueryHandler : IRequestHandler<GetPaymentForMerchantsWithPaginationQuery, ApiResponse<PaginatedList<PaymentDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;
    private readonly ICurrentUser _currentUser;

    public GetPaymentForMerchantsWithPaginationQueryHandler(
        IOrderDbContext context, IMapper mapper,
        IIdentityClient identityClient, ICatalogClient catalogClient, ICurrentUser currentUser
        )
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
        _currentUser = currentUser;
    }
    public async Task<ApiResponse<PaginatedList<PaymentDto>>> Handle(GetPaymentForMerchantsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;

        var query = _context.Payment.Where(x => !x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.ToLower().Contains(s.Amount.ToString().ToLower())
                            || paramSearchText.ToLower().Contains(s.FullName.ToString().ToLower())
                            || paramSearchText.ToLower().Contains(s.Phone.ToString().ToLower())
                            || paramSearchText.ToLower().Contains(s.Email.ToString().ToLower())
                            || paramSearchText.ToLower().Contains(s.BookingId.ToString().ToLower())
                            );
        }
        List<long> storeIds = new List<long>();
        try
        {
            var response = (await _catalogClient.GetUserStoreByUserIdAsync(_currentUser.UserId))?.Data;
            if (response != null)
            {
                storeIds = response.Select(u => u.StoreId).Distinct().ToList();
            }
        }
        catch (Exception ex) { }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .Include(x => x.Booking)
            .Where(p => storeIds.Contains((long)p.Booking.StoreId))
            .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<PaymentDto>>.Success(paginationResult);
    }
}