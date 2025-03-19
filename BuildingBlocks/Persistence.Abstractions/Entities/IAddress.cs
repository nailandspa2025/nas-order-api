namespace BuildingBlocks.Persistence.Abstractions.Entities;

public interface IAddress
{
    int? ProvinceId { get; }

    int? DistrictId { get; }

    int? WardId { get; }

    string? Address { get; }
}

