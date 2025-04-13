using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ShareBook.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockExams.Infra.AwsS3;

public class AwsS3Service : IAwsS3Service
{
    private AwsS3Settings _settings { get; set; }
    private IAmazonS3 _client { get; set; }

    public AwsS3Service(IOptions<AwsS3Settings> settings)
    {
        _settings = settings.Value;

        if (_settings.IsActive)
        {
            _client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, Amazon.RegionEndpoint.SAEast1);
        }
    }

    public async Task SendFile(string filePath, string remoteFolder)
    {
        if (!_settings.IsActive) throw new AwsS3DisabledException("O serviço AWS S3 está desativado no appsettings.");

        var fileName = Path.GetFileName(filePath);
        PutObjectRequest request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = remoteFolder + "/" + fileName,
            FilePath = filePath
        };

        await _client.PutObjectAsync(request);
    }

    public async Task<string> GetFileTmpUrl(string fullPath)
    {
        if (!_settings.IsActive) throw new AwsS3DisabledException("O serviço AWS S3 está desativado no appsettings.");

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _settings.BucketName,
            Key = fullPath,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        var url = _client.GetPreSignedURL(request);
        return url;
    }
}
