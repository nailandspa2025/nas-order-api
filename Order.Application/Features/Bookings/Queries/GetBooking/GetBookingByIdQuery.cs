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
            if (entity.TechnicianId.HasValue)
            {
                var technicianRespnse = await _identityClient.GetTechnicianByIdAsync(entity.TechnicianId.Value, cancellationToken);
                if (technicianRespnse?.Data != null)
                {
                    bookingDto.Technician = technicianRespnse.Data;
                }
            }
        }
        catch (Exception ){}
        try
        {
           //if(entity.ServiceId.HasValue)
           // {
           //     var serviceResponse = await _catalogClient.GetServiceIdAsync(entity.ServiceId.Value, cancellationToken);
           //     if(serviceResponse?.Data != null)
           //     {
           //         bookingDto.Service = serviceResponse.Data;
           //     }
           // }
        }
        catch (Exception){}

        return ApiResponse<BookingDto>.Success(bookingDto);
    }
}
