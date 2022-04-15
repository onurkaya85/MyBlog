﻿using MyBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Entities.Concrete
{
    public class Comment: BaseEntity,IBaseEntity
    {
        public string Text { get; set; }
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}
