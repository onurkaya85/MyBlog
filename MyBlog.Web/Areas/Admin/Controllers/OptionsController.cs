using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.Entities.Concrete;
using MyBlog.Mvc.Areas.Admin.Models;
using MyBlog.Services.Abstract;
using MyBlog.Shared.Utilities.Helpers.Abstract;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OptionsController : Controller
    {
        private readonly AboutUsPageInfo _aboutUsPageInfo;
        private readonly IWritableOptions<AboutUsPageInfo> _aboutUsPageInfoWriteable;
        private readonly IToastNotification _toastNotification;
        private readonly WebsiteInfo _webSiteInfo;
        private readonly IWritableOptions<WebsiteInfo> _websiteInfoWriteable;
        private readonly SmtpSettings _smtpSettings;
        private readonly IWritableOptions<SmtpSettings> _smtpSettingsWriteable;
        private readonly ArticleRightSideBarWidgetOptions _articleRightSideBarWidgetOptions;
        private readonly IWritableOptions<ArticleRightSideBarWidgetOptions> _articleRightSideBarWidgetOptionsWriteable;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public OptionsController(IOptionsSnapshot<AboutUsPageInfo> aboutUsPageInfo,
            IWritableOptions<AboutUsPageInfo> aboutUsPageInfoWriteable,
            IToastNotification toastNotification,
            IOptionsSnapshot<WebsiteInfo> webSiteInfo,
            IWritableOptions<WebsiteInfo> websiteInfoWriteable,
            IOptionsSnapshot<SmtpSettings> smtpSettings,
            IWritableOptions<SmtpSettings> smtpSettingsWriteable,
            IOptionsSnapshot<ArticleRightSideBarWidgetOptions> articleRightSideBarWidgetOptions,
            IWritableOptions<ArticleRightSideBarWidgetOptions> articleRightSideBarWidgetOptionsWriteable,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _aboutUsPageInfo = aboutUsPageInfo.Value;
            _aboutUsPageInfoWriteable = aboutUsPageInfoWriteable;
            _toastNotification = toastNotification;
            _webSiteInfo = webSiteInfo.Value;
            _websiteInfoWriteable = websiteInfoWriteable;
            _smtpSettings = smtpSettings.Value;
            _smtpSettingsWriteable = smtpSettingsWriteable;
            _articleRightSideBarWidgetOptions = articleRightSideBarWidgetOptions.Value;
            _articleRightSideBarWidgetOptionsWriteable = articleRightSideBarWidgetOptionsWriteable;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult About()
        {
            return View(_aboutUsPageInfo);
        }

        [HttpPost]
        public IActionResult About(AboutUsPageInfo aboutUsPageInfo)
        {
            if(ModelState.IsValid)
            {
                _aboutUsPageInfoWriteable.Update(x =>
                {
                    x.Header = aboutUsPageInfo.Header;
                    x.Content = aboutUsPageInfo.Content;
                    x.SeoAuthor = aboutUsPageInfo.SeoAuthor;
                    x.SeoDescription = aboutUsPageInfo.SeoDescription;
                    x.SeoTags = aboutUsPageInfo.SeoTags;
                });

                _toastNotification.AddSuccessToastMessage("Hakkınızda sayfa içerikleri güncellenmiştir", new ToastrOptions
                {
                    Title = "Başarılı İşlem"
                });
            }
            return View(aboutUsPageInfo);
        }

        [HttpGet]
        public IActionResult GeneralSettings()
        {
            return View(_webSiteInfo);
        }
        [HttpPost]
        public IActionResult GeneralSettings(WebsiteInfo webSiteInfo)
        {
            if (ModelState.IsValid)
            {
                _websiteInfoWriteable.Update(x =>
                {
                    x.Title = webSiteInfo.Title;
                    x.MenuTitle = webSiteInfo.MenuTitle;
                    x.SeoAuthor = webSiteInfo.SeoAuthor;
                    x.SeoDescription = webSiteInfo.SeoDescription;
                    x.SeoTags = webSiteInfo.SeoTags;
                });

                _toastNotification.AddSuccessToastMessage("Sitenizin genel ayarları güncellenmiştir", new ToastrOptions
                {
                    Title = "Başarılı İşlem"
                });
            }
            return View(webSiteInfo);
        }

        [HttpGet]
        public IActionResult EmailSettings()
        {
            return View(_smtpSettings);
        }
        [HttpPost]
        public IActionResult EmailSettings(SmtpSettings smtpSettings)
        {
            if (ModelState.IsValid)
            {
                _smtpSettingsWriteable.Update(x =>
                {
                    x.Server = smtpSettings.Server;
                    x.Port = smtpSettings.Port;
                    x.SenderName = smtpSettings.SenderName;
                    x.SenderEmail = smtpSettings.SenderEmail;
                    x.Username = smtpSettings.Username;
                    x.Password = smtpSettings.Password;
                });

                _toastNotification.AddSuccessToastMessage("Sitenizin email ayarları güncellenmiştir", new ToastrOptions
                {
                    Title = "Başarılı İşlem"
                });
            }
            return View(smtpSettings);
        }

        [HttpGet]
        public async Task<IActionResult> ArticleRightSideBarWidgetSettings()
        {
            var categoriesResult = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            var articleRightSideBarWidgetOptionsViewModel = _mapper.Map<ArticleRightSideBarWidgetOptionsViewModel>(_articleRightSideBarWidgetOptions);
            articleRightSideBarWidgetOptionsViewModel.Categories = categoriesResult.Data.Categories;
            return View(articleRightSideBarWidgetOptionsViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ArticleRightSideBarWidgetSettings(ArticleRightSideBarWidgetOptionsViewModel articleRightSideBarWidgetOptionsViewModel)
        {
            var categoriesResult = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            articleRightSideBarWidgetOptionsViewModel.Categories = categoriesResult.Data.Categories;
            if (ModelState.IsValid)
            {
                _articleRightSideBarWidgetOptionsWriteable.Update(x =>
                {
                    x.Header = articleRightSideBarWidgetOptionsViewModel.Header;
                    x.TakeSize = articleRightSideBarWidgetOptionsViewModel.TakeSize;
                    x.CategoryId = articleRightSideBarWidgetOptionsViewModel.CategoryId;
                    x.FilterBy = articleRightSideBarWidgetOptionsViewModel.FilterBy;
                    x.OrderBy = articleRightSideBarWidgetOptionsViewModel.OrderBy;
                    x.IsAscending = articleRightSideBarWidgetOptionsViewModel.IsAscending;
                    x.StartAt = articleRightSideBarWidgetOptionsViewModel.StartAt;
                    x.EndAt = articleRightSideBarWidgetOptionsViewModel.EndAt;
                    x.MaxViewCount = articleRightSideBarWidgetOptionsViewModel.MaxViewCount;
                    x.MinViewCount = articleRightSideBarWidgetOptionsViewModel.MinViewCount;
                    x.MaxCommentCount = articleRightSideBarWidgetOptionsViewModel.MaxCommentCount;
                    x.MinCommentCount = articleRightSideBarWidgetOptionsViewModel.MinCommentCount;
                });

                _toastNotification.AddSuccessToastMessage("Makale sayfalarınızın widget ayarları başarıyla güncellenmiştir.", new ToastrOptions
                {
                    Title = "Başarılı İşlem!"
                });
                return View(articleRightSideBarWidgetOptionsViewModel);
            }
            return View(articleRightSideBarWidgetOptionsViewModel);
        }
    }
}
