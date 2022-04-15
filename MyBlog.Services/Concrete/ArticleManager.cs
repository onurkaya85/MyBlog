using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data.Abstract;
using MyBlog.Entities.ComplexTypes;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.Dtos;
using MyBlog.Services.Abstract;
using MyBlog.Services.Utilities;
using MyBlog.Shared.Entities.Concrete;
using MyBlog.Shared.Utilities.Results.Abstract;
using MyBlog.Shared.Utilities.Results.ComplexType;
using MyBlog.Shared.Utilities.Results.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Services.Concrete
{
    public class ArticleManager : ManagerBase, IArticleService
    {
        private readonly UserManager<User> _userManager;

        public ArticleManager(IUnitOfWork uow, IMapper mapper, UserManager<User> userManager) : base(uow, mapper)
        {
            _userManager = userManager;
        }

        public async Task<IDataResult<ArticleDto>> GetAsync(int articleId)
        {
            var article = await _uow.Articles.GetAsync(v => v.Id == articleId, c => c.User, c => c.Category);

            if (article != null)
            {
                article.Comments = await _uow.Comments.GetAllAsync(c => c.ArticleId == articleId && !c.IsDeleted && c.IsActive);
                return new DataResult<ArticleDto>(ResultStatus.Success, data: new ArticleDto
                {
                    Article = article,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleDto>(ResultStatus.Error, message: Messages.Article.NotFound(false), data: null);
        }

        public async Task<IDataResult<ArticleDto>> GetByIdAsync(int articleId, bool includeCategory, bool includeComments, bool includeUser)
        {
            List<Expression<Func<Article, bool>>> predicates = new List<Expression<Func<Article, bool>>>();
            List<Expression<Func<Article, object>>> includes = new List<Expression<Func<Article, object>>>();

            if (includeCategory)
                includes.Add(a => a.Category);
            if (includeComments)
                includes.Add(a => a.Comments);
            if (includeUser)
                includes.Add(a => a.User);

            predicates.Add(a => a.Id == articleId);
            
            var article = await _uow.Articles.GetAsyncV2(predicates, includes);
            if (article == null)
            {
                return new DataResult<ArticleDto>(ResultStatus.Warning, message: Messages.General.ValidationError(), data: null, new List<ValidationError>
                {
                    new ValidationError
                    {
                       PropertyName = "articleId",
                       Message = Messages.Article.NotFoundById(articleId)
                    }
                });
            }
            return new DataResult<ArticleDto>(ResultStatus.Success, data: new ArticleDto
            {
                Article = article
            });
        }

        public async Task<IDataResult<ArticleUpdateDto>> GetArticleUpdateDtoAsync(int articleId)
        {
            var result = await _uow.Articles.AnyAsync(c => c.Id == articleId);
            if (result)
            {
                var article = await _uow.Articles.GetAsync(v => v.Id == articleId);
                var articleUpdateDto = _mapper.Map<ArticleUpdateDto>(article);
                return new DataResult<ArticleUpdateDto>(ResultStatus.Success, data: articleUpdateDto);
            }
            else
                return new DataResult<ArticleUpdateDto>(ResultStatus.Error, message: Messages.Category.NotFound(false), data: null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllAsync()
        {
            var articles = await _uow.Articles.GetAllAsync(null, a => a.User, c => c.Category);
            if (articles.Any())
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }

            return new DataResult<ArticleListDto>(ResultStatus.Error, message: Messages.Article.NotFound(true), null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllAsyncV2(int? categoryId, int? userId, bool? isActive, bool? isDeleted, int? currentPage, int? pageSize, OrderByGeneral? orderBy, bool? isAscending, bool includeCategory, bool includeComments, bool includeUser)
        {
            var predicates = new List<Expression<Func<Article, bool>>>();
            var includes = new List<Expression<Func<Article, object>>>();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                if (!await _uow.Categories.AnyAsync(c => c.Id == categoryId.Value))
                {
                    return new DataResult<ArticleListDto>(ResultStatus.Warning, message: Messages.General.ValidationError(), data: null, new List<ValidationError>
                    {
                        new ValidationError
                        {
                           PropertyName = "categoryId",
                           Message = Messages.Category.NotFoundById(categoryId.Value)
                        }
                    });
                }
                predicates.Add(a => a.CategoryId == categoryId.Value);
            }

            if (userId.HasValue && userId.Value > 0)
            {
                if (!await _userManager.Users.AnyAsync(c => c.Id == userId.Value))
                {
                    return new DataResult<ArticleListDto>(ResultStatus.Warning, message: Messages.General.ValidationError(), data: null, new List<ValidationError>
                    {
                        new ValidationError
                        {
                           PropertyName = "userId",
                           Message = Messages.User.NotFoundById(userId.Value)
                        }
                    }); ;
                }
                predicates.Add(a => a.UserId == userId.Value);
            }

            if (isActive.HasValue)
                predicates.Add(v => v.IsActive == isActive.Value);

            if (isDeleted.HasValue)
                predicates.Add(v => v.IsDeleted == isDeleted.Value);

            if (includeCategory)
                includes.Add(a => a.Category);
            if (includeComments)
                includes.Add(a => a.Comments);
            if (includeUser)
                includes.Add(a => a.User);

            var articles = await _uow.Articles.GetAllAsyncV2(predicates, includes);
            IOrderedEnumerable<Article> sortedArticles = null;
            if (orderBy.HasValue)
            {

                switch (orderBy)
                {
                    case OrderByGeneral.Id:
                        sortedArticles = (isAscending.HasValue && isAscending.Value == true)
                            ? articles.OrderBy(v => v.Id)
                            : articles.OrderByDescending(v => v.Id);
                        break;
                    case OrderByGeneral.Az:
                        sortedArticles = (isAscending.HasValue && isAscending.Value == true)
                            ? articles.OrderBy(v => v.Title)
                            : articles.OrderByDescending(v => v.Title);
                        break;
                    case OrderByGeneral.CreateDate:
                        sortedArticles = (isAscending.HasValue && isAscending.Value == true)
                            ? articles.OrderBy(v => v.CreatedDate)
                            : articles.OrderByDescending(v => v.CreatedDate);
                        break;
                    default:
                        sortedArticles = (isAscending.HasValue && isAscending.Value == true)
                            ? articles.OrderBy(v => v.CreatedDate)
                            : articles.OrderByDescending(v => v.CreatedDate);
                        break;
                }
            }

            //Aslında sayfalama için ayrı metod yapılmalı ve bu parametreler oraya alınmalı

            if (!currentPage.HasValue)
                currentPage = 1;
            if (!pageSize.HasValue)
                pageSize = 10;


            return new DataResult<ArticleListDto>(ResultStatus.Success, new ArticleListDto
            {
                Articles = sortedArticles.Skip((currentPage.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList(),
                CategoryId = categoryId.HasValue ? categoryId.Value : null,
                CurrentPage = currentPage.Value,
                PageSize = pageSize.Value,
                IsAcsending = isAscending.Value,
                TotalCount = articles.Count,
                ResultStatus = ResultStatus.Success
            });
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByNonDeletedAsync()
        {
            var articles = await _uow.Articles.GetAllAsync(v => !v.IsDeleted, a => a.User, a => a.Category);
            if (articles.Any())
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, Messages.Article.NotFound(true), null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByNoneDeletedAndActiveAsync()
        {
            var articles = await _uow.Articles.GetAllAsync(v => !v.IsDeleted && v.IsActive, a => a.User, a => a.Category);
            if (articles.Any())
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, message: Messages.Article.NotFound(true), null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByCategoryAsync(int categoryId)
        {
            var catResult = await _uow.Categories.AnyAsync(v => v.Id == categoryId);

            if (catResult)
            {
                var articles = await _uow.Articles.GetAllAsync(v => v.CategoryId == categoryId && !v.IsDeleted && v.IsActive, a => a.User, a => a.Category);
                if (articles.Any())
                {
                    return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
                    {
                        Articles = articles,
                        ResultStatus = ResultStatus.Success
                    });
                }
                return new DataResult<ArticleListDto>(ResultStatus.Error, message: Messages.Article.NotFound(true), null);
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, message: Messages.Category.NotFound(true), null);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByViewCountAsync(bool isAscending, int? takeSize)
        {
            var articles = await _uow.Articles.GetAllAsync(c => c.IsActive && !c.IsDeleted, c => c.Category, c => c.User);
            var sortedArticles = isAscending
                ? articles.OrderBy(v => v.ViewsCount)
                : articles.OrderByDescending(v => v.ViewsCount);

            return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
            {
                Articles = (takeSize.HasValue && takeSize.Value > 0) ? sortedArticles.Take(takeSize.Value).ToList() : sortedArticles.ToList()
            });
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByPagingAsync(int? categoryId, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;

            var articles = categoryId == null
                ? await _uow.Articles.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.Category, a => a.User)
                : await _uow.Articles.GetAllAsync(a => a.CategoryId == categoryId.Value && a.IsActive && !a.IsDeleted, a => a.Category, a => a.User);

            var sortedArticles = isAscending
                ? articles.OrderBy(v => v.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(v => v.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
            {
                Articles = sortedArticles,
                CategoryId = categoryId == null ? null : categoryId,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = articles.Count,
                IsAcsending = isAscending
            });
        }

        public async Task<IResult> AddAsync(ArticleAddDto articleAddDto, string createdByName, int userId)
        {
            var article = _mapper.Map<Article>(articleAddDto);

            article.CreatedByName = createdByName;
            article.ModifiedByName = createdByName;
            article.UserId = userId;

            await _uow.Articles.AddAsync(article);

            int result = await _uow.SaveAsync();

            if (result > 0)
                return new Result(ResultStatus.Success, message: $"{articleAddDto.Title} başlıklı makale eklenmiştir");

            return new Result(ResultStatus.Error, message: $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz");
        }

        public async Task<IResult> UpdateAsync(ArticleUpdateDto articleUpdateDto, string modifiedByName)
        {
            var oldArticle = await _uow.Articles.GetAsync(v => v.Id == articleUpdateDto.Id);

            var article = _mapper.Map<ArticleUpdateDto, Article>(articleUpdateDto, oldArticle);
            article.ModifiedByName = modifiedByName;

            await _uow.Articles.UpdateAsync(article);
            await _uow.SaveAsync();

            return new Result(ResultStatus.Success, message: $"{articleUpdateDto.Title} başlıklı makale güncellenmiştir");

        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var articlesCount = await _uow.Articles.CountAsync();
            if (articlesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, articlesCount);
            }
            return new DataResult<int>(ResultStatus.Error, message: Messages.Article.NotFound(true), -1);
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var articlesCount = await _uow.Articles.CountAsync(v => !v.IsDeleted);
            if (articlesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, articlesCount);
            }
            return new DataResult<int>(ResultStatus.Error, message: Messages.Article.NotFound(true), -1);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByDeletedAsync()
        {
            var articles = await _uow.Articles.GetAllAsync(v => !v.IsDeleted, a => a.User, a => a.Category);
            if (articles.Any())
            {
                return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
                {
                    Articles = articles,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<ArticleListDto>(ResultStatus.Error, message: Messages.Article.NotFound(true), null);
        }

        public async Task<IResult> DeleteAsync(int articleId, string modifiedByName)
        {
            var result = await _uow.Articles.AnyAsync(c => c.Id == articleId);
            if (result)
            {
                var article = await _uow.Articles.GetAsync(v => v.Id == articleId);
                article.ModifiedByName = modifiedByName;
                article.ModifiedDate = DateTime.Now;
                article.IsDeleted = true;
                article.IsActive = false;

                await _uow.Articles.UpdateAsync(article);
                await _uow.SaveAsync();
                return new Result(ResultStatus.Success, message: $"{article.Title} başlıklı makale silinmiştir");
            }

            return new Result(ResultStatus.Error, message: $"Makale bulunamadı");
        }

        public async Task<IResult> HardDeleteAsync(int articleId)
        {
            var result = await _uow.Articles.AnyAsync(c => c.Id == articleId);
            if (result)
            {
                var article = await _uow.Articles.GetAsync(v => v.Id == articleId);
                await _uow.Articles.DeleteAsync(article).ContinueWith(t => _uow.SaveAsync());
                return new Result(ResultStatus.Success, message: $"{article.Title} başlıklı makale silinmiştir");
            }

            return new Result(ResultStatus.Error, message: $"Makale bulunamadı");
        }

        public async Task<IResult> UndoDeleteAsync(int articleId, string modifiedByName)
        {
            var result = await _uow.Articles.AnyAsync(c => c.Id == articleId);
            if (result)
            {
                var article = await _uow.Articles.GetAsync(v => v.Id == articleId);
                article.ModifiedByName = modifiedByName;
                article.ModifiedDate = DateTime.Now;
                article.IsDeleted = false;
                article.IsActive = true;

                await _uow.Articles.UpdateAsync(article);
                await _uow.SaveAsync();
                return new Result(ResultStatus.Success, message: Messages.Article.UndoDelete(article.Title));
            }

            return new Result(ResultStatus.Error, message: Messages.Article.NotFound(isPlural: false));
        }

        public async Task<IDataResult<ArticleListDto>> SearchAsync(string keyword, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var articles = await _uow.Articles.GetAllAsync(a => a.IsActive && !a.IsDeleted, a => a.Category, a => a.User);

                var sortedArticles = isAscending
                    ? articles.OrderBy(v => v.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                    : articles.OrderByDescending(v => v.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
                {
                    Articles = sortedArticles,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalCount = articles.Count,
                    IsAcsending = isAscending
                });
            }
            var searchArticles = await _uow.Articles.SearchAsync(new List<Expression<Func<Article, bool>>>
            {
                (a) => a.Title.Contains(keyword),
                (a) => a.Category.Name.Contains(keyword),
                (a) => a.SeoDescription.Contains(keyword),
                (a) => a.SeoTags.Contains(keyword)
            }, a => a.Category, a => a.User);

            var searchSortedArticles = isAscending
                   ? searchArticles.OrderBy(v => v.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                   : searchArticles.OrderByDescending(v => v.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
            {
                Articles = searchSortedArticles,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = searchArticles.Count,
                IsAcsending = isAscending
            });
        }

        public async Task<IResult> IncreaseViewCountAsync(int articleId)
        {
            var article = await _uow.Articles.GetAsync(v => v.Id == articleId);
            if (article == null)
                return new Result(ResultStatus.Error, message: Messages.Article.NotFound(isPlural: false));

            article.ViewsCount += 1;
            await _uow.Articles.UpdateAsync(article);
            await _uow.SaveAsync();

            return new Result(ResultStatus.Success, message: Messages.Article.IncreaseViewCount(article.Title));
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByUserIdOnFilter(int userId, FilterBy filterBy, OrderBy orderBy, bool isAcsending, int takeSize, int categoryId, DateTime startAt, DateTime endAt, int minViewCount, int maxViewCount, int minCommentCount, int maxCommentCount)
        {
            var anyUser = await _userManager.Users.AnyAsync(v => v.Id == userId);
            if (!anyUser)
            {
                return new DataResult<ArticleListDto>(ResultStatus.Error, message: $"{userId} ID'li kullanıcı bulunamadı", data: null);
            }

            var userArticles = await _uow.Articles.GetAllAsync(v => v.IsActive && !v.IsDeleted && v.UserId == userId);
            var sortedArticleList = new List<Article>();
            switch (filterBy)
            {
                case FilterBy.Category:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderBy(c => c.Date).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderBy(c => c.ViewsCount).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.ViewsCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderBy(c => c.CommentCount).ToList()
                               : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.CommentCount).ToList();
                            break;
                    }
                    break;
                case FilterBy.Date:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.Date >= startAt && v.Date <= endAt).Take(takeSize).OrderBy(c => c.Date).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.Date >= startAt && v.Date <= endAt).Take(takeSize).OrderBy(c => c.ViewsCount).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.ViewsCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.Date >= startAt && v.Date <= endAt).Take(takeSize).OrderBy(c => c.CommentCount).ToList()
                               : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.CommentCount).ToList();
                            break;
                    }
                    break;
                case FilterBy.ViewCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.ViewsCount >= minViewCount && v.ViewsCount <= maxViewCount).Take(takeSize).OrderBy(c => c.Date).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.ViewsCount >= minViewCount && v.ViewsCount <= maxViewCount).Take(takeSize).OrderBy(c => c.ViewsCount).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.ViewsCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.ViewsCount >= minViewCount && v.ViewsCount <= maxViewCount).Take(takeSize).OrderBy(c => c.CommentCount).ToList()
                               : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.CommentCount).ToList();
                            break;
                    }
                    break;
                case FilterBy.CommentCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.ViewsCount >= minCommentCount && v.ViewsCount <= maxCommentCount).Take(takeSize).OrderBy(c => c.Date).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.Date).ToList();
                            break;
                        case OrderBy.ViewCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.ViewsCount >= minCommentCount && v.ViewsCount <= maxCommentCount).Take(takeSize).OrderBy(c => c.ViewsCount).ToList()
                                : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.ViewsCount).ToList();
                            break;
                        case OrderBy.CommentCount:
                            sortedArticleList = isAcsending ? userArticles.Where(v => v.ViewsCount >= minCommentCount && v.ViewsCount <= maxCommentCount).Take(takeSize).OrderBy(c => c.CommentCount).ToList()
                               : userArticles.Where(v => v.CategoryId == categoryId).Take(takeSize).OrderByDescending(c => c.CommentCount).ToList();
                            break;
                    }
                    break;
            }

            return new DataResult<ArticleListDto>(ResultStatus.Success, data: new ArticleListDto
            {
                Articles = sortedArticleList
            });
        }


    }
}
