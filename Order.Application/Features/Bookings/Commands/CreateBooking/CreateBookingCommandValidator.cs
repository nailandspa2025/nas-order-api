using FluentValidation;

namespace Order.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator: AbstractValidator<CreateBookingCommand>
{
   
    public CreateBookingCommandValidator()
    {
        
    }
}