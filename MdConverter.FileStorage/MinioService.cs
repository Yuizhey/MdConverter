using MdConverter.Application;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

public class MinioService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _minioSettings;

    public MinioService(IOptions<MinioSettings> minioSettings)
    {
        _minioSettings = minioSettings.Value;
        _minioClient = new MinioClient()
            .WithEndpoint(_minioSettings.Endpoint)
            .WithCredentials(_minioSettings.AccessKey, _minioSettings.SecretKey);
    }

    public async Task UploadFileAsync(string fileName, Stream fileStream)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("Invalid file stream");
        }

        fileStream.Position = 0;
        const string bucketName = "usersdocuments";
        

        Console.WriteLine($"Checking bucket '{bucketName}'...");
        var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));

        if (!bucketExists)
        {
            Console.WriteLine($"Bucket '{bucketName}' does not exist. Creating...");
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }

        Console.WriteLine($"Uploading file '{fileName}'...");
        var response = await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length));

        Console.WriteLine($"File '{fileName}' uploaded successfully.");
    }

    public async Task<Stream> DownloadFileAsync(string userName, string fileName)
    {
        var filePath = $"{userName}/{fileName}";
        var fileStream = new MemoryStream();

        Console.WriteLine($"Downloading file '{filePath}'...");
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_minioSettings.BucketName)
            .WithObject(filePath)
            .WithCallbackStream((stream) => stream.CopyTo(fileStream)));

        fileStream.Position = 0;
        return fileStream;
    }

    public async Task DeleteFileAsync(string userName, string fileName)
    {
        var filePath = $"{userName}/{fileName}";
        Console.WriteLine($"Deleting file '{filePath}'...");
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_minioSettings.BucketName)
            .WithObject(filePath));
    }
}
