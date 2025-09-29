using AutoMapper;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Domain.Entities;

namespace Order.Application.Features.Bookings.Commands.UpdateBooking;

public record UpdateBookingCommand: IRequest<ApiResponse<BookingDto>>
{
    public int Id { get; init; }

    public long? StoreId { get; init; }

    public long? ProductId { get; init; }

    public long? TechnicianId { get; init; }

    public DateTime BookingDate { get; init; }

    public TimeSpan BookingTime { get; init; }

    public string? Note { get; set; }

    public string? FullName { get; init; }

    public string? Address { get; init; }

    public Gender? Gender { get; init; }

    public string? Phone { get; init; }

    public string? Email { get; init; }

    public int? Number { get; init; }
}

public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public UpdateBookingCommandHandler(
        IMapper mapper,
        IOrderDbContext context,
        
        IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        
        _storageService = storageService;
    }
    public async Task<ApiResponse<BookingDto>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .FindAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }
        if (entity.Status != BookingStatus.Pending)
        {
            return ApiResponse<BookingDto>.Error("Only update bookings with status pending.");
        }
        entity.StoreId = entity.StoreId;
        entity.ProductId = request.ProductId;
        //entity.TechnicianId = request.TechnicianId;
        entity.BookingTime = request.BookingTime;
        entity.Note = request.Note;
        entity.FullName = request.FullName;
        entity.Address = request.Address;
        entity.Gender = request.Gender;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.Number = request.Number;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}