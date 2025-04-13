using System.Threading.Tasks;

namespace MockExams.Infra.AwsS3
{
    public interface IAwsS3Service
    {
        Task<string> GetFileTmpUrl(string fullPath);
        Task SendFile(string filePath, string remoteFolder);
    }
}