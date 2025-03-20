using Amazon.S3;
using Amazon.S3.Model;
using StreamingVideoWebApi.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using StreamingVideoWebApi.Infra.Settings;

namespace StreamingVideoWebApi.Infra.Services;

public class S3StorageService : IS3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Config _s3Config;
    public S3StorageService(IAmazonS3 s3Client, IOptions<S3Config> s3Config)
    {
        _s3Client = s3Client;
        _s3Config = s3Config.Value;
    }
    public string SignRemoteFileUrl(string key)
    {
       try
       {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = _s3Config.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddHours(1), // TODO: Define the expiration time on appsettings
            };
            return _s3Client.GetPreSignedURL(request);
        }
       catch (Exception ex)
       {
            throw new Exception($"Error while signing the remote file url for key {key}", ex);

       }
    }
}
