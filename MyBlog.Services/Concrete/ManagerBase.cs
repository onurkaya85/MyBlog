using AutoMapper;
using MyBlog.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Services.Concrete
{
    public class ManagerBase
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IMapper _mapper;

        public ManagerBase(IUnitOfWork uow,IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
    }
}
