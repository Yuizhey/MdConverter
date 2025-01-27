using MdConverter.Application;
using Minio;
using Minio.DataModel.Args;

namespace MdConverter.FileStorage;

public class MinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioSettings _minioSettings;

        public MinioService(MinioSettings minioSettings)
        {
            _minioSettings = minioSettings;
            _minioClient = new MinioClient()
                .WithEndpoint(_minioSettings.Endpoint)
                .WithCredentials(_minioSettings.AccessKey, _minioSettings.SecretKey);
        }

        public async Task UploadFileAsync(string userName, string fileName, Stream fileStream)
        {
            var filePath = $"{userName}/{fileName}";
            var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.BucketName));

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minioSettings.BucketName));
            }

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_minioSettings.BucketName)
                .WithObject(filePath)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length));
        }

        public async Task<Stream> DownloadFileAsync(string userName, string fileName)
        {
            var filePath = $"{userName}/{fileName}";
            var fileStream = new MemoryStream();
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
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_minioSettings.BucketName)
                .WithObject(filePath));
        }
    }