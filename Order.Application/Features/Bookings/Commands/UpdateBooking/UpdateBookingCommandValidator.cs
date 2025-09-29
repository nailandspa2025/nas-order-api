
using FluentValidation;
namespace Order.Application.Features.Bookings.Commands.UpdateBooking;

public class UpdateBookingCommandValidator :AbstractValidator<UpdateBookingCommand>
{
	public UpdateBookingCommandValidator()
	{
    }
}

