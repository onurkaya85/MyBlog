using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MyBlog.Data.Abstract;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.Dtos;
using MyBlog.Services.Abstract;
using MyBlog.Services.Utilities;
using MyBlog.Shared.Utilities.Results.Abstract;
using MyBlog.Shared.Utilities.Results.ComplexType;
using MyBlog.Shared.Utilities.Results.Concrete;

namespace MyBlog.Services.Concrete
{
    public class CommentManager : ManagerBase, ICommentService
    {
        public CommentManager(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public async Task<IDataResult<CommentDto>> GetAsync(int commentId)
        {
            var comment = await _uow.Comments.GetAsync(c => c.Id == commentId);
            if (comment != null)
            {
                return new DataResult<CommentDto>(ResultStatus.Success, new CommentDto
                {
                    Comment = comment,
                });
            }
            return new DataResult<CommentDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: false), new CommentDto
            {
                Comment = null,
            });
        }

        public async Task<IDataResult<CommentUpdateDto>> GetCommentUpdateDtoAsync(int commentId)
        {
            var result = await _uow.Comments.AnyAsync(c => c.Id == commentId);
            if (result)
            {
                var comment = await _uow.Comments.GetAsync(c => c.Id == commentId);
                var commentUpdateDto = _mapper.Map<CommentUpdateDto>(comment);
                return new DataResult<CommentUpdateDto>(ResultStatus.Success, commentUpdateDto);
            }
            else
            {
                return new DataResult<CommentUpdateDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: false), null);
            }
        }

        public async Task<IDataResult<CommentListDto>> GetAllAsync()
        {
            var comments = await _uow.Comments.GetAllAsync(null,c=> c.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(ResultStatus.Success, new CommentListDto
                {
                    Comments = comments
                });
            }
            return new DataResult<CommentListDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: true), new CommentListDto
            {
                Comments = null,
            });
        }

        public async Task<IDataResult<CommentListDto>> GetAllByDeletedAsync()
        {
            var comments = await _uow.Comments.GetAllAsync(c => c.IsDeleted,includeProperties: v=> v.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(ResultStatus.Success, new CommentListDto
                {
                    Comments = comments
                });
            }
            return new DataResult<CommentListDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: true), new CommentListDto
            {
                Comments = null,
            });
        }

        public async Task<IDataResult<CommentListDto>> GetAllByNonDeletedAsync()
        {
            var comments = await _uow.Comments.GetAllAsync(c => !c.IsDeleted, includeProperties: v => v.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(ResultStatus.Success, new CommentListDto
                {
                    Comments = comments
                });
            }
            return new DataResult<CommentListDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: true), new CommentListDto
            {
                Comments = null,
            });
        }

        public async Task<IDataResult<CommentListDto>> GetAllByNonDeletedAndActiveAsync()
        {
            var comments = await _uow.Comments.GetAllAsync(c => !c.IsDeleted && c.IsActive, includeProperties: v => v.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(ResultStatus.Success, new CommentListDto
                {
                    Comments = comments
                });
            }
            return new DataResult<CommentListDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: true), new CommentListDto
            {
                Comments = null,
            });
        }

        public async Task<IDataResult<CommentDto>> AddAsync(CommentAddDto commentAddDto)
        {
            var comment = _mapper.Map<Comment>(commentAddDto);
            var article = await _uow.Articles.GetAsync(v => v.Id == commentAddDto.ArticleId);
            if(article == null)
            {
                return new DataResult<CommentDto>(ResultStatus.Error, Messages.Article.NotFound(isPlural: false), data: null);
            }
            var addedComment = await _uow.Comments.AddAsync(comment);

            article.CommentCount += 1; 
            await _uow.Articles.UpdateAsync(article);
            await _uow.SaveAsync();
            return new DataResult<CommentDto>(ResultStatus.Success, Messages.Comment.Add(commentAddDto.CreatedByName), new CommentDto
            {
                Comment = addedComment
            });
        }

        public async Task<IDataResult<CommentDto>> UpdateAsync(CommentUpdateDto commentUpdateDto, string modifiedByName)
        {
            var oldComment = await _uow.Comments.GetAsync(c => c.Id == commentUpdateDto.Id);
            var comment = _mapper.Map<CommentUpdateDto, Comment>(commentUpdateDto, oldComment);
            comment.ModifiedByName = modifiedByName;
            var updatedComment = await _uow.Comments.UpdateAsync(comment);
            updatedComment.Article = await _uow.Articles.GetAsync(v => v.Id == updatedComment.ArticleId);
            await _uow.SaveAsync();
            return new DataResult<CommentDto>(ResultStatus.Success, Messages.Comment.Update(comment.CreatedByName), new CommentDto
            {
                Comment = updatedComment
            });
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var commentsCount = await _uow.Comments.CountAsync();
            if (commentsCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, commentsCount);
            }
            else
            {
                return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı.", -1);
            }
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var commentsCount = await _uow.Comments.CountAsync(c => !c.IsDeleted);
            if (commentsCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, commentsCount);
            }
            else
            {
                return new DataResult<int>(ResultStatus.Error, $"Beklenmeyen bir hata ile karşılaşıldı.", -1);
            }
        }

        public async Task<IDataResult<CommentDto>> AproveAsync(int commentId,string modifiedByName)
        {
            var comment = await _uow.Comments.GetAsync(v => v.Id == commentId,includeProperties:v=> v.Article);
            if(comment != null)
            {
                var article = comment.Article;
                comment.IsActive = true;
                comment.ModifiedByName = modifiedByName;
                comment.ModifiedDate = DateTime.Now;

                var updatedComment = await _uow.Comments.UpdateAsync(comment);
                article.CommentCount = await _uow.Comments.CountAsync(v => v.ArticleId == article.Id && !v.IsDeleted);
                await _uow.Articles.UpdateAsync(article);
                await _uow.SaveAsync();
                return new DataResult<CommentDto>(ResultStatus.Success, message: Messages.Comment.Aprove(commentId), data: new CommentDto
                { 
                   Comment = updatedComment
                });
            }

            return new DataResult<CommentDto>(ResultStatus.Error, message: Messages.Comment.NotFound(isPlural:false), data: null);
        }

        public async Task<IDataResult<CommentDto>> DeleteAsync(int commentId, string modifiedByName)
        {
            var comment = await _uow.Comments.GetAsync(c => c.Id == commentId,includeProperties:c=> c.Article);
            if (comment != null)
            {
                var article = comment.Article;
                comment.IsDeleted = true;
                comment.IsActive = false;
                comment.ModifiedByName = modifiedByName;
                comment.ModifiedDate = DateTime.Now;
                var deletedComment = await _uow.Comments.UpdateAsync(comment);
                article.CommentCount -= 1;
                await _uow.Articles.UpdateAsync(article);
                await _uow.SaveAsync();
                return new DataResult<CommentDto>(ResultStatus.Success, Messages.Comment.Delete(deletedComment.CreatedByName), new CommentDto
                {
                    Comment = deletedComment,
                });
            }
            return new DataResult<CommentDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: false), new CommentDto
            {
                Comment = null,
            });
        }

        public async Task<IResult> HardDeleteAsync(int commentId)
        {
            var comment = await _uow.Comments.GetAsync(c => c.Id == commentId,c=> c.Article);
            if (comment != null)
            {
                if(comment.IsDeleted)
                {
                    await _uow.Comments.DeleteAsync(comment);
                    await _uow.SaveAsync();
                    return new Result(ResultStatus.Success, Messages.Comment.HardDelete(comment.CreatedByName));
                }
                var article = comment.Article;
                await _uow.Comments.DeleteAsync(comment);
                article.CommentCount = await _uow.Comments.CountAsync(c => c.ArticleId == article.Id && !c.IsDeleted);
                await _uow.Articles.UpdateAsync(article);
                await _uow.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Comment.HardDelete(comment.CreatedByName));
            }
            return new Result(ResultStatus.Error, Messages.Comment.NotFound(isPlural: false));
        }

        public async Task<IDataResult<CommentDto>> UndoDeleteAsync(int commentId, string modifiedByName)
        {
            var comment = await _uow.Comments.GetAsync(c => c.Id == commentId, c => c.Article);
            if (comment != null)
            {
                var article = comment.Article;
                comment.IsDeleted = false;
                comment.IsActive = true;
                comment.ModifiedByName = modifiedByName;
                comment.ModifiedDate = DateTime.Now;

                var deletedComment = await _uow.Comments.UpdateAsync(comment);
                article.CommentCount += 1; 
                await _uow.Articles.UpdateAsync(article);
                await _uow.SaveAsync();

                return new DataResult<CommentDto>(ResultStatus.Success, Messages.Comment.UndoDelete(deletedComment.CreatedByName), new CommentDto
                {
                    Comment = deletedComment,
                });
            }
            return new DataResult<CommentDto>(ResultStatus.Error, Messages.Comment.NotFound(isPlural: false), new CommentDto
            {
                Comment = null,
            });
        }
    }
}
