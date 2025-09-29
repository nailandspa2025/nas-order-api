using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Bookings.Queries.GetBooking;

public record GetBookingByIdQuery: IRequest<ApiResponse<BookingDto>>
{
    public long Id { get; init; }
}

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;

    public GetBookingByIdQueryHandler(IOrderDbContext context, IMapper mapper, IIdentityClient identityClient, ICatalogClient catalogClient)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
    }
    public async Task<ApiResponse<BookingDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .Include(x => x.BookingServices)
            .Include(x => x.BookingTechnicians)
            .Include(x => x.BookingSnaps)
            .Include(x => x.BookingSnapGroups)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }
        var bookingDto = _mapper.Map<BookingDto>(entity);
        try
        {   if(entity.StoreId.HasValue)
            {
                var storeResponse = await _catalogClient.GetStoreByIdAsync(entity.StoreId.Value, cancellationToken);
                if (storeResponse?.Data != null)
                {
                    bookingDto.Store = storeResponse.Data;
                }
            }
        }
        catch (Exception ex){}
        try
        {
            var technicianIds = entity.BookingTechnicians.Select(x => x.TechnicianId).ToList();
            if (technicianIds.Any())
            {
                var technicianResponse = await _identityClient.GetTechnicianByIdsAsync(string.Join(",", technicianIds), cancellationToken);
                if (technicianResponse?.Data != null)
                {
                    bookingDto.Technicians = technicianResponse.Data.ToList();
                }
            }
        }
        catch (Exception ){}
        try
        {
            var serviceIds = entity.BookingServices.Select(bs => bs.ServiceId).ToList();
            if (serviceIds.Any())
            {
                var serviceResponse = await _catalogClient.GetServiceIdsAsync(string.Join(",", serviceIds), cancellationToken);
                if (serviceResponse?.Data != null)
                {
                    bookingDto.Services = serviceResponse.Data.ToList();
                }
            }
        }
        catch (Exception){}

        return ApiResponse<BookingDto>.Success(bookingDto);
    }
}
