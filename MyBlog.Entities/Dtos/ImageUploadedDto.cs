using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Entities.Dtos
{
    public class ImageUploadedDto
    {
        public string FullName { get; set; }
        public string OldName { get; set; }
        /// <summary>
        /// resmin uzantısı .png,.jpeg gibi
        /// </summary>
        public string Extension { get; set; }
        public string Path { get; set; }
        public string FolderName { get; set; }
        public long Size { get; set; }
    }
}
