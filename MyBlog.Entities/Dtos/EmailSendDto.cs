using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Entities.Dtos
{
    public class EmailSendDto
    {
        [DisplayName("İsminiz")]
        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        [MaxLength(60,ErrorMessage ="{0} alanı en fazla {1} karakter olabilir")]
        [MinLength(5, ErrorMessage = "{0} alanı en az {1} karakter olabilir")]
        public string Name { get; set; }

        [DisplayName("E-Posta Adresiniz")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        [MaxLength(100, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir")]
        [MinLength(10, ErrorMessage = "{0} alanı en az {1} karakter olabilir")]
        public string Email { get; set; }

        [DisplayName("Konu")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        [MaxLength(125, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir")]
        [MinLength(5, ErrorMessage = "{0} alanı en az {1} karakter olabilir")]
        public string Subject { get; set; }

        [DisplayName("Mesajınız")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        [MaxLength(1500, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir")]
        [MinLength(20, ErrorMessage = "{0} alanı en az {1} karakter olabilir")]
        public string Message { get; set; }
    }
}
