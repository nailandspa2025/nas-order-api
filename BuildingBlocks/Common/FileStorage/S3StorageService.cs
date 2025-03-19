using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.Contracts;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;


namespace BuildingBlocks.Common.FileStorage;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _amazonS3Client;
    private readonly string _bucketName;
    private readonly string _publicEndpoint;

    public S3StorageService(IConfiguration configuration)
    {
        var regionEndpointName = configuration["AWS:S3:RegionEndpointName"];
        var accessKeyId = configuration["AWS:S3:AccessKeyId"];
        var secretAccessKey = configuration["AWS:S3:SecretAccessKey"];
        _bucketName = configuration["AWS:S3:BucketName"];

        _publicEndpoint = configuration["AWS:S3:PublicEndpoint"];

        Contract.Requires(string.IsNullOrWhiteSpace(regionEndpointName));
        Contract.Requires(string.IsNullOrWhiteSpace(accessKeyId));
        Contract.Requires(string.IsNullOrWhiteSpace(secretAccessKey));
        Contract.Requires(string.IsNullOrWhiteSpace(_bucketName));

        _amazonS3Client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(regionEndpointName));

        if (string.IsNullOrWhiteSpace(_publicEndpoint))
        {
            _publicEndpoint = $"http://s3.{regionEndpointName}.amazonaws.com/{_bucketName}/";
        }
    }

    public async Task<ApiResponse> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
        };

        await _amazonS3Client.DeleteObjectAsync(deleteObjectRequest, cancellationToken);
        return ApiResponse.Success();
    }

    public string GetFileUrl(string fileName)
    {
        return string.Concat(_publicEndpoint, fileName);
    }

    public async Task<string> SaveFileAsync(Stream mediaBinaryStream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            CannedACL = S3CannedACL.PublicRead,
            InputStream = mediaBinaryStream
        };

        var transferUtility = new TransferUtility(_amazonS3Client);
        await transferUtility.UploadAsync(uploadRequest, cancellationToken);
        return GetFileUrl(fileName);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            CannedACL = S3CannedACL.PublicRead,
            InputStream = file.OpenReadStream()
        };

        var transferUtility = new TransferUtility(_amazonS3Client);
        await transferUtility.UploadAsync(uploadRequest, cancellationToken);
        return GetFileUrl(fileName);
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

