using System.IO;
using System.Threading.Tasks;

namespace VideoGameGrade.Services
{
    public interface IAzureBlobStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName);
    }
}