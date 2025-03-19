using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Common.FileStorage;

public interface IStorageService
{
    string GetFileUrl(string fileName);

    Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName, CancellationToken cancellationToken = default);

    Task<string> SaveFileAsync(IFormFile file, string fileName, CancellationToken cancellationToken = default);

    Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default);

    Task<string> SaveFileAsync(string base64String, CancellationToken cancellationToken = default);

    Task<List<string>> SaveFilesAsync(List<string> base64String, CancellationToken cancellationToken = default);

    Task<List<string>> SaveFilesAsync(List<IFormFile> files, CancellationToken cancellationToken = default);

    Task<ApiResponse> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
}

