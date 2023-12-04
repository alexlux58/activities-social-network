namespace Application.Photos
{
    public class PhotoUploadResult
    {
        public string PublicId { get; set; } // this is the id of the photo in cloudinary
        public string Url { get; set; } // this is the url of the photo in cloudinary
        
    }
}