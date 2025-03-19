namespace Order.Domain.Enums;


public enum BookingStatus
{
    Pending = 1,
    Completed = 2,
    Cancelled = 3
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Refunded = 3
}

public enum PaymentMethod
{
    Cash = 1, 
    CreditCard = 2,
    Momo = 3,
    Zalopay= 4
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}