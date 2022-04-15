using MyBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Entities.Concrete
{
    public class Category : BaseEntity, IBaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
