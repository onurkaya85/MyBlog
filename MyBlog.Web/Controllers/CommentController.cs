using Microsoft.AspNetCore.Mvc;
using MyBlog.Entities.Dtos;
using MyBlog.Services.Abstract;
using MyBlog.Shared.Utilities.Extensions;
using MyBlog.Web.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyBlog.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public Task<string> RenderViewToString { get; private set; }

        [HttpPost]
        public async Task<IActionResult> Add(CommentAddDto commentAddDto)
        {
            if(ModelState.IsValid)
            {
                var result = await _commentService.AddAsync(commentAddDto);
                if(result.ResultStatus == Shared.Utilities.Results.ComplexType.ResultStatus.Success)
                {
                    var commentAddJsonModel = JsonSerializer.Serialize(new CommentAddAjaxViewModel
                    {
                        CommentDto = result.Data,
                        CommentAddPartial = await this.RenderViewToStringAsync("_CommentAddPartial", commentAddDto)
                    }, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    });

                    return Json(commentAddJsonModel);
                }
                ModelState.AddModelError("", result.Message);
            }

            var commentAddJsonErrorModel = JsonSerializer.Serialize(new CommentAddAjaxViewModel
            {
                CommentAddDto = commentAddDto,
                CommentAddPartial = await this.RenderViewToStringAsync("_CommentAddPartial", commentAddDto)
            });

            return Json(commentAddJsonErrorModel);
        }
    }
}
