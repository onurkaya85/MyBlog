﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Entities.Concrete;
using MyBlog.Services.Abstract;
using MyBlog.Shared.Utilities.Results.ComplexType;
using MyBlog.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;

        public HomeController(ICategoryService categoryService, IArticleService articleService, ICommentService commentService,UserManager<User> userManager)
        {
            _categoryService = categoryService;
            _articleService = articleService;
            _commentService = commentService;
            _userManager = userManager;
        }

        [Authorize(Roles ="SuperAdmin,AdminArea.Home.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var catCount = await _categoryService.CountByNonDeletedAsync();
            var articlesCount = await _articleService.CountByNonDeletedAsync();
            var commentsCount = await _commentService.CountByNonDeletedAsync();
            var usersCount = await _userManager.Users.CountAsync();

            var articles = await _articleService.GetAllAsync();

            if(catCount.ResultStatus == ResultStatus.Success && articlesCount.ResultStatus == ResultStatus.Success 
                && commentsCount.ResultStatus == ResultStatus.Success && usersCount > -1 && articles.ResultStatus == ResultStatus.Success)
            {
               return View(new DashboardViewModel
                {
                    CategoriesCount = catCount.Data,
                    ArticlesCount = articlesCount.Data,
                    CommentsCount = commentsCount.Data,
                    UserCount = usersCount,
                    Articles = articles.Data
                });
            }
            return NotFound();
        }
    }
}
