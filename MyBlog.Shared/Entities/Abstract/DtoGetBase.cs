using MyBlog.Shared.Utilities.Results.ComplexType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Shared.Entities.Abstract
{
    public abstract class DtoGetBase
    {
        public virtual ResultStatus ResultStatus { get; set; }
        public virtual string Message { get; set; }
        public virtual int CurrentPage { get; set; } = 1;
        public virtual int PageSize { get; set; } = 5;
        public virtual int TotalCount { get; set; }
        public virtual bool IsAcsending { get; set; } = false;

        public virtual int TotalPages
        {
            get
            {
                return (int)Math.Ceiling(decimal.Divide(TotalCount, PageSize));
            }
        }

        public virtual bool ShowPrevious
        {
            get
            {
                return CurrentPage > 1;
            }
        }

        public virtual bool ShowNext
        {
            get
            {
                return CurrentPage < TotalPages;
            }
        }
    }
}
