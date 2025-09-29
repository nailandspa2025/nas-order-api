using FluentValidation;

namespace Order.Application.Features.Bookings.Queries.GetBookingsWithPagination;

public class GetBookingsWithPaginationQueryValidattor: AbstractValidator<GetBookingsWithPaginationQuery>
{
	public GetBookingsWithPaginationQueryValidattor()
	{
        RuleFor(x => x.PageNumber)
           .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}