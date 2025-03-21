using Amazon.S3;
using Amazon.S3.Model;
using StreamingVideoWebApi.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using StreamingVideoWebApi.Infra.Settings;
using Microsoft.Extensions.Caching.Distributed;

namespace StreamingVideoWebApi.Infra.Services;

public class S3StorageService : IS3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Config _s3Config;
    private readonly IDistributedCache _cache;
    public S3StorageService(IAmazonS3 s3Client, IOptions<S3Config> s3Config, IDistributedCache cache)
    {
        _s3Client = s3Client;
        _s3Config = s3Config.Value;
        _cache = cache;
    }
    public string SignRemoteFileUrl(string key)
    {
       try
       {
            var cacheKey = $"SignedUrl-{key}";
            var cachedUrl = _cache.GetString(cacheKey);
            if (cachedUrl is not null)
            {
                return cachedUrl;
            }

            var request = new GetPreSignedUrlRequest()
            {
                BucketName = _s3Config.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddHours(1), // TODO: Define the expiration time on appsettings
                Protocol = Protocol.HTTP
            };
            var mediaUrl = _s3Client.GetPreSignedURL(request);
            CacheUrl(cacheKey, mediaUrl);
            return mediaUrl;
        }
       catch (Exception ex)
       {
            throw new Exception($"Error while signing the remote file url for key {key}", ex);

       }
    }

    private void CacheUrl(string key, string url)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // TODO: Move the expiration time to a configuration file
        };
        _cache.SetString(key, url, cacheOptions);
    }
}
