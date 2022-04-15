using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.Dtos;
using MyBlog.Services.Abstract;
using MyBlog.Shared.Utilities.Helpers.Abstract;
using NToastNotify;
using System;
using System.Threading.Tasks;

namespace MyBlog.Web.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly AboutUsPageInfo _aboutUsPageInfo;
        private readonly IMailService _mailService;
        private readonly IToastNotification _toastNotification;
        private readonly IWritableOptions<AboutUsPageInfo> _aboutUsPageInfoWritable;

        public HomeController(IArticleService articleService,IOptionsSnapshot<AboutUsPageInfo> aboutUsPageInfo, IWritableOptions<AboutUsPageInfo> aboutUsPageInfoWritable)
        {
            _articleService = articleService;
            _aboutUsPageInfo = aboutUsPageInfo.Value;
            _aboutUsPageInfoWritable = aboutUsPageInfoWritable;
        }

        [Route("index")]
        [Route("anasayfa")]
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId, int currentPage = 1, int pageSize = 5,bool isAcsending = false)
        {
            var articlesResult = await (!categoryId.HasValue
                ? _articleService.GetAllByPagingAsync(null,currentPage: currentPage, pageSize: pageSize,isAscending:isAcsending)
                : _articleService.GetAllByPagingAsync(categoryId: categoryId, currentPage: currentPage, pageSize: pageSize, isAscending: isAcsending));

            return View(articlesResult.Data);
        }

        [Route("hakkımızda")]
        [Route("hakkinda")]
        [HttpGet]
        public IActionResult About()
        {
            //içindeki değeri değiştirmek istiyorsan
            //_aboutUsPageInfoWritable.Update(x =>
            //{
            //    x.Header = "Yeni MyBlogBaşlık";
            //    x.Content = "Yeni İçerik";
            //});
            return View(_aboutUsPageInfo); //okumak için yeterli
        }

        [Route("iletişim")]
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("iletişim")]
        [HttpPost]
        public IActionResult Contact(EmailSendDto emailSendDto)
        {
            if (ModelState.IsValid)
            {
                var result = _mailService.SendContactEmail(emailSendDto);
                _toastNotification.AddSuccessToastMessage(result.Message, new ToastrOptions
                {
                    Title = "Başarılı İşlem!"
                });
                return View();

            }
            return View(emailSendDto);
        }
    }
}
