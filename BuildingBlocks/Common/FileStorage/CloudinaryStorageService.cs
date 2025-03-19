using System.Security.AccessControl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.Common.FileStorage;

public class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _bucketName;

    public CloudinaryStorageService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _cloudinary = new Cloudinary(new Account
        {
            ApiKey = configuration["CloudinarySettings:ApiKey"],
            ApiSecret = configuration["CloudinarySettings:ApiSecret"],
            Cloud = configuration["CloudinarySettings:Cloud"]
        });

        _bucketName = configuration["CloudinarySettings:BucketName"] ?? webHostEnvironment.EnvironmentName;
    }

    public async Task<ApiResponse> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var deletionParams = new DeletionParams(fileName)
        {
            ResourceType = CloudinaryDotNet.Actions.ResourceType.Raw
        };

        var destroyResult = await _cloudinary.DestroyAsync(deletionParams);
        return destroyResult.Result == "ok" ? ApiResponse.Success() : ApiResponse.Error(destroyResult.Error.Message);
    }

    public string GetFileUrl(string fileName)
    {
        return _cloudinary.Api.UrlImgUp
            .Secure(true)
            .BuildUrl(string.Concat(_bucketName, "/", fileName));
    }

    public async Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, mediaBinaryStream),
            PublicId = fileName,
            Folder = _bucketName
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        return result.SecureUrl.ToString();
    }

    public async Task<string> SaveFileAsync(IFormFile file, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, file.OpenReadStream()),
            PublicId = fileName,
            Folder = _bucketName
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        return result.SecureUrl.ToString();
    }

    public async Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var fileName = Guid.NewGuid().ToString("N");
        return await SaveFileAsync(file, $"{fileName}", cancellationToken);
    }

    public async Task<string> SaveFileAsync(string base64String, CancellationToken cancellationToken = default)
    {
        var imageBytes = Convert.FromBase64String(base64String);
        var stream = new MemoryStream(imageBytes);
        var fileName = Guid.NewGuid();
        return await SaveFileAsync(stream, $"{fileName}", cancellationToken);
    }

    public async Task<List<string>> SaveFilesAsync(List<string> base64String, CancellationToken cancellationToken = default)
    {
        var tasks = base64String.Select(e => SaveFileAsync(e, cancellationToken));
        await Task.WhenAll(tasks);
        return tasks.Select(e => e.Result).ToList();
    }

    public async Task<List<string>> SaveFilesAsync(List<IFormFile> files, CancellationToken cancellationToken = default)
    {
        var tasks = files.Select(e => SaveFileAsync(e, cancellationToken));
        await Task.WhenAll(tasks);
        return tasks.Select(e => e.Result).ToList();
    }
}
