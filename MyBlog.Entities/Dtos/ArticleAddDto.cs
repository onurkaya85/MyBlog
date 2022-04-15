using MyBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Entities.Dtos
{
    public class ArticleAddDto
    {
        [DisplayName("Başlık")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        [MaxLength(100,ErrorMessage ="{0} alanı {1} karakterden büyük olamaz")]
        [MinLength(5, ErrorMessage = "{0} alanı {1} karakterden küçük olamaz")]
        public string Title { get; set; }

        [DisplayName("İçerik")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        [MinLength(10, ErrorMessage = "{0} alanı {1} karakterden küçük olamaz")]
        public string Content { get; set; }

        [DisplayName("Thumbnail")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        [MaxLength(250, ErrorMessage = "{0} alanı {1} karakterden büyük olamaz")]
        [MinLength(5, ErrorMessage = "{0} alanı {1} karakterden küçük olamaz")]
        public string Thumbnail { get; set; }

        [DisplayName("Tarih")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [DisplayName("Seo Yazar")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        [MaxLength(50, ErrorMessage = "{0} alanı {1} karakterden büyük olamaz")]
        [MinLength(0, ErrorMessage = "{0} alanı {1} karakterden küçük olamaz")]
        public string SeoAuther { get; set; }

        [DisplayName("Seo Açıklama")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        [MaxLength(150, ErrorMessage = "{0} alanı {1} karakterden büyük olamaz")]
        [MinLength(0, ErrorMessage = "{0} alanı {1} karakterden küçük olamaz")]
        public string SeoDescription { get; set; }

        [DisplayName("Seo Etiketler")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        [MaxLength(70, ErrorMessage = "{0} alanı {1} karakterden büyük olamaz")]
        [MinLength(0, ErrorMessage = "{0} alanı {1} karakterden küçük olamaz")]
        public string SeoTags { get; set; }

        [DisplayName("Kategori")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        public int CategoryId { get; set; }

        [DisplayName("Aktif mi?")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez.")]
        public bool IsActive { get; set; }
        
        public Category Category { get; set; }

    }
}
