using AutoMapper;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Services.AutoMapper.Profiles
{
    public class ArticleProfile: Profile
    {
        public ArticleProfile()
        {
            CreateMap<ArticleAddDto, Article>()
                .ForMember(dest=> dest.CreatedDate, op => op.MapFrom(x=> DateTime.Now));

            CreateMap<ArticleUpdateDto, Article>()
                .ForMember(dest=> dest.ModifiedDate,op=> op.MapFrom(x=> DateTime.Now));

            CreateMap<Article, ArticleUpdateDto>();
        }
    }
}
