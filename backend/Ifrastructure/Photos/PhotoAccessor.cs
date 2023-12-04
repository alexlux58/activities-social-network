using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Ifrastructure.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            
            _cloudinary = new Cloudinary(account);
        }
        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            if(file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream), // this is the file that we are uploading
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill") // this is the size of the photo that we are uploading
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if(result.Error != null)
                {
                    throw new Exception(result.Error.Message);
                }

                return new PhotoUploadResult
                {
                    PublicId = result.PublicId, // this is the id of the photo in cloudinary
                    Url = result.SecureUrl.ToString() // this is the url of the photo in cloudinary
                };
            }

            return null;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId); // this is the id of the photo in cloudinary

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok" ? result.Result : null;
        }
    }
}