using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Entities.Dtos
{
    public class CategoryUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [DisplayName("Kategori Adı")]
        [Required(ErrorMessage = "{0} boş geçilemez")]
        [MaxLength(75, ErrorMessage = "{0} {1} karakterden fazla olamaz.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden küçük olamaz.")]
        public string Name { get; set; }

        [DisplayName("Kategori Açıklaması")]
        [MaxLength(500, ErrorMessage = "{0} {1} karakterden fazla olamaz.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden küçük olamaz.")]
        public string Description { get; set; }

        [DisplayName("Kategori Özel Not Alanı ")]
        [MaxLength(50, ErrorMessage = "{0} {1} karakterden fazla olamaz.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden küçük olamaz.")]
        public string Note { get; set; }

        [DisplayName("Aktif mi?")]
        [Required(ErrorMessage = "{0} boş geçilemez")]
        public bool IsActive { get; set; }

        [DisplayName("Silinsin mi?")]
        [Required(ErrorMessage = "{0} boş geçilemez")]
        public bool IsDeleted { get; set; }
    }
}
