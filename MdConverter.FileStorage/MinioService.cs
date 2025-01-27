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
            _minioClient = new MinioClient()
                .WithEndpoint(configuration["MinioSettings:Endpoint"])
                .WithCredentials(configuration["MinioSettings:AccessKey"], configuration["MinioSettings:SecretKey"])
                .Build();

            _bucketName = configuration["MinioSettings:BucketName"];
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
}
