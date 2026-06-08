
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Core_Layer.Services.ImageServices;

public class ImageServices(IWebHostEnvironment environment)
{
    public string[] Extensions
    {
        get { return new[]{ ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg", ".ico",
            ".tiff", ".tif", ".heic", ".heif", ".avif",".jfif" }; }
    }

    public async Task<string> SaveImageAndGiveName(IFormFile image)
    {
        var guid = Guid.NewGuid();
        var imagename = image.FileName + '_' + guid.ToString();
        var path = Path.Combine(environment.ContentRootPath, "Uploads",imagename);        
        var fileStream = new FileStream(path,FileMode.CreateNew);
        await using (fileStream)
        {
           await image.CopyToAsync(fileStream);
        }

        return imagename;
    }

    public bool ValidateExtension(string imageName)
    {
        string extention = Path.GetExtension(imageName);
        if (extention == "")
            return false;
       
        if(Extensions.Contains(extention)) 
        { 
            return true;
        }

        return false;
    }
}