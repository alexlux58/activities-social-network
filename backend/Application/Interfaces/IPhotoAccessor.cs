using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        Task<PhotoUploadResult> AddPhoto(IFormFile file); // IFormFile is a file that is being uploaded
        Task<string> DeletePhoto(string publicId);
        
    }
}