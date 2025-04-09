namespace BuildingBlocks.CommonAuthorization;

public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(string permissionName);
}

