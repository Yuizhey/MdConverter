using MdConverter.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

public class MinioService
{
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;

        public MinioService(IConfiguration configuration)
        {
            var endpoint = configuration["MINIO:URL"];
            var accessKey = configuration["MINIO:ACCESSKEY"];
            var secretKey = configuration["MINIO:SECRETKEY"];
            _bucketName = configuration["MINIO:BUCKETNAME"];

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentException("MINIO:URL не задан в переменных окружения.");
            if (string.IsNullOrEmpty(accessKey))
                throw new ArgumentException("MINIO:ACCESSKEY не задан в переменных окружения.");
            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentException("MINIO:SECRETKEY не задан в переменных окружения.");
            if (string.IsNullOrEmpty(_bucketName))
                throw new ArgumentException("MINIO:BUCKETNAME не задан в переменных окружения.");

            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build();
        }

        public async Task<bool> UploadFileAsync(string objectName, Stream fileStream)
        {
            if (fileStream == null || fileStream.Length == 0)
            {
                throw new ArgumentException("Invalid file stream");
            }

            try
            {
                // Ensure the bucket exists
                bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
                if (!bucketExists)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
                }

                // Upload the file
                fileStream.Position = 0;

                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType("application/octet-stream"));

                Console.WriteLine($"File '{objectName}' uploaded successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return false;
            }
        }

        public async Task<Stream> DownloadFileAsync(string objectName)
        {
            try
            {
                var memoryStream = new MemoryStream();

                await _minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

                memoryStream.Position = 0;
                Console.WriteLine($"File '{objectName}' downloaded successfully.");
                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving file: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string objectName)
        {
            try
            {
                objectName += ".md";
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName));

                Console.WriteLine($"File '{objectName}' deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> FileExistsAsync(string objectName)
        {
            try
            {
                var exists = await _minioClient.StatObjectAsync(new StatObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName));
                return exists != null; // Если файл найден, возвращаем true
            }
            catch (Exception)
            {
                return false; // Если файл не найден, выбрасывается исключение, возвращаем false
            }
        }
}
