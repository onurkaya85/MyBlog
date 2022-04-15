using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyBlog.Entities.ComplexTypes;
using MyBlog.Entities.Dtos;
using MyBlog.Shared.Utilities.Extensions;
using MyBlog.Shared.Utilities.Results.Abstract;
using MyBlog.Shared.Utilities.Results.ComplexType;
using MyBlog.Shared.Utilities.Results.Concrete;
using MyBlog.Web.Helpers.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyBlog.Web.Helpers.Concrete
{
    public class ImageHelper : IImageHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly string wwwRoot;
        private readonly string imgFolder = "img";
        private const string userImagesFolder = "userImages";
        private const string postImagesFolder = "postImages";

        public ImageHelper(IWebHostEnvironment env)
        {
            _env = env;
            wwwRoot = _env.WebRootPath;
        }

        public async Task<IDataResult<ImageUploadedDto>> Upload(string name, IFormFile pictureFile, PictureType pictureType, string folderName = null)
        {
            folderName = (!string.IsNullOrEmpty(folderName)) 
                ? folderName 
                : (pictureType == PictureType.User ? userImagesFolder : postImagesFolder);

            if (!Directory.Exists($"{wwwRoot}/{imgFolder}/{folderName}"))
                Directory.CreateDirectory($"{wwwRoot}/{imgFolder}/{folderName}");

            string oldFileName = Path.GetFileNameWithoutExtension(pictureFile.FileName);
            string fileExtesion = Path.GetExtension(pictureFile.FileName);

            Regex regex = new Regex("[*'\",._&#^@]");
            name = regex.Replace(name, "_"); 

            DateTime dateTime = DateTime.Now;
            string newFileName = $"{name}_{dateTime.FullDateAndTimeStringWithUnderscore()}{fileExtesion}";
            string path = Path.Combine($"{wwwRoot}/{imgFolder}/{folderName}", newFileName);

            await using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await pictureFile.CopyToAsync(fs);
            }

            string message = pictureType == PictureType.User 
                ? $"{name} kullanıcısının resmi yüklendi." 
                : $"{name} adlı makalenin resmi yüklenmiştir";

            return new DataResult<ImageUploadedDto>(ResultStatus.Success, message, new ImageUploadedDto
            {
                FullName = $"{folderName}/{newFileName}",
                OldName = oldFileName,
                Extension = fileExtesion,
                FolderName = folderName,
                Path = path,
                Size = pictureFile.Length
            });
        }

        public IDataResult<ImageDeletedDto> Delete(string pictureName)
        {
            string fileToDelete = Path.Combine($"{wwwRoot}/{imgFolder}/", pictureName);

            if (System.IO.File.Exists(fileToDelete))
            {
                var fileInfo = new FileInfo(fileToDelete);
                var imageDeletedDto = new ImageDeletedDto
                {
                    FullName = pictureName,
                    Extension = fileInfo.Extension,
                    Path = fileInfo.FullName,
                    Size = fileInfo.Length
                };

                System.IO.File.Delete(fileToDelete);
                return new DataResult<ImageDeletedDto>(ResultStatus.Success, imageDeletedDto);
            }
            return new DataResult<ImageDeletedDto>(ResultStatus.Error, message: $"Böyle bir resim bulunamadı", null);
        }
    }
}
