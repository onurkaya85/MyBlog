using AutoMapper;
using MyBlog.Data.Abstract;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.Dtos;
using MyBlog.Services.Abstract;
using MyBlog.Services.Utilities;
using MyBlog.Shared.Utilities.Results.Abstract;
using MyBlog.Shared.Utilities.Results.ComplexType;
using MyBlog.Shared.Utilities.Results.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Services.Concrete
{
    public class CategoryManager : ManagerBase,ICategoryService
    {
        public CategoryManager(IUnitOfWork uow, IMapper mapper):base(uow,mapper)
        {
            
        }

        public async Task<IDataResult<CategoryDto>> GetAsync(int categoryId)
        {
            var category = await _uow.Categories.GetAsync(v => v.Id == categoryId, includeProperties: v => v.Articles);
            if (category != null)
            {
                return new DataResult<CategoryDto>(ResultStatus.Success, new CategoryDto
                {
                    Category = category,
                    ResultStatus = ResultStatus.Success
                });
            }

            return new DataResult<CategoryDto>(ResultStatus.Error, message: Messages.Category.NotFound(isPlural:false), data: new CategoryDto
            {
                Category = null,
                Message = Messages.Category.NotFound(isPlural: false),
                ResultStatus = ResultStatus.Error
            });
        }

        public async Task<IDataResult<CategoryListDto>> GetAllAsync()
        {
            var categories = await _uow.Categories.GetAllAsync(null, c => c.Articles);

            if (categories.Any())
            {
                return new DataResult<CategoryListDto>(ResultStatus.Success, new CategoryListDto
                {
                    Categories = categories,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<CategoryListDto>(ResultStatus.Error, message: Messages.Category.NotFound(true), data: new CategoryListDto
            {
                Categories = null,
                ResultStatus = ResultStatus.Error,
                Message = Messages.Category.NotFound(true)
            });
        }

        public async Task<IDataResult<CategoryListDto>> GetAllByNonDeletedAsync()
        {
            var categories = await _uow.Categories.GetAllAsync(c => !c.IsDeleted, v => v.Articles);

            if (categories.Any())
            {
                return new DataResult<CategoryListDto>(ResultStatus.Success, new CategoryListDto
                {
                    Categories = categories,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<CategoryListDto>(ResultStatus.Error, message: Messages.Category.NotFound(true), data: new CategoryListDto
            {
                Categories = null,
                ResultStatus = ResultStatus.Error,
                Message = Messages.Category.NotFound(true)
            });
        }

        public async Task<IDataResult<CategoryListDto>> GetAllByNonDeletedAndActiveAsync()
        {
            var categories = await _uow.Categories.GetAllAsync(c => !c.IsDeleted && c.IsActive, v => v.Articles);

            if (categories.Any())
            {
                return new DataResult<CategoryListDto>(ResultStatus.Success, new CategoryListDto
                {
                    Categories = categories,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<CategoryListDto>(ResultStatus.Error, message: Messages.Category.NotFound(true), data: null);
        }

        public async Task<IDataResult<CategoryDto>> AddAsync(CategoryAddDto categoryAddDto, string createdByName)
        {
            var category = _mapper.Map<Category>(categoryAddDto);
            category.CreatedByName = createdByName;
            category.ModifiedByName = createdByName;

            var addedCategory = await _uow.Categories.AddAsync(category);

            if (await _uow.SaveAsync() > 0)
            {
                return new DataResult<CategoryDto>(ResultStatus.Success, message: Messages.Category.Add(addedCategory.Name), data: new CategoryDto
                {
                    Category = addedCategory,
                    ResultStatus = ResultStatus.Success,
                    Message = Messages.Category.Add(addedCategory.Name)
                });
            }

            return new DataResult<CategoryDto>(ResultStatus.Error, message: $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz", data: new CategoryDto
            {
                Category = addedCategory,
                ResultStatus = ResultStatus.Error,
                Message = $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz"
            });
        }

        public async Task<IDataResult<CategoryDto>> UpdateAsync(CategoryUpdateDto categoryUpdateDto, string modifiedByName)
        {
            var oldCategory = await _uow.Categories.GetAsync(v => v.Id == categoryUpdateDto.Id);

            var category = _mapper.Map<CategoryUpdateDto,Category>(categoryUpdateDto, oldCategory);
            category.ModifiedByName = modifiedByName;

            var updatedCategory = await _uow.Categories.UpdateAsync(category);

            if (await _uow.SaveAsync() > 0)
            {
                return new DataResult<CategoryDto>(ResultStatus.Success, message: Messages.Category.Update(updatedCategory.Name), data: new CategoryDto
                {
                    Category = updatedCategory,
                    ResultStatus = ResultStatus.Success,
                    Message = Messages.Category.Update(updatedCategory.Name)
                });
            }
            return new DataResult<CategoryDto>(ResultStatus.Error, message: $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz", data: new CategoryDto
            {
                Category = updatedCategory,
                ResultStatus = ResultStatus.Error,
                Message = $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz"
            });
        }

        public async Task<IDataResult<CategoryDto>> DeleteAsync(int categoryId, string modifiedByName)
        {
            var category = await _uow.Categories.GetAsync(v => v.Id == categoryId);
            if (category != null)
            {
                category.IsDeleted = true;
                category.IsActive = false;
                category.ModifiedByName = modifiedByName;
                category.ModifiedDate = DateTime.Now;

                var deletedCategory = await _uow.Categories.UpdateAsync(category);

                if (await _uow.SaveAsync() > 0)
                {
                    return new DataResult<CategoryDto>(ResultStatus.Success, message: $"{category.Name} adlı Kategori silindi", data: new CategoryDto
                    {
                        Category = deletedCategory,
                        ResultStatus = ResultStatus.Success,
                        Message = Messages.Category.Delete(deletedCategory.Name)
                    });
                }
                return new DataResult<CategoryDto>(ResultStatus.Error, message: $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz", data: new CategoryDto
                {
                    Category = deletedCategory,
                    ResultStatus = ResultStatus.Error,
                    Message = $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz"
                });
            }

            return new DataResult<CategoryDto>(ResultStatus.Error, message: Messages.Category.NotFound(false), data: new CategoryDto
            {
                Category = null,
                ResultStatus = ResultStatus.Error,
                Message = Messages.Category.NotFound(false)
            });
        }

        public async Task<IResult> HardDeleteAsync(int categoryId)
        {
            var category = await _uow.Categories.GetAsync(v => v.Id == categoryId);
            if (category != null)
            {
                await _uow.Categories.DeleteAsync(category);
                await _uow.SaveAsync();
                return new Result(ResultStatus.Success, message: Messages.Category.HardDelete(category.Name));
            }

            return new Result(ResultStatus.Error, message: Messages.Category.NotFound(false));
        }

        /// <summary>
        /// Verilen ID parametsine ait kategorinin CategoryUpdateDto temsilini geriye döner
        /// </summary>
        /// <param name="categoryId">0'dan büyük int bir Id değeri</param>
        /// <returns>Asenkron olarak Task şeklinde ve DataResult tipinde döner.</returns>
        public async Task<IDataResult<CategoryUpdateDto>> GetCategoryUpdateDtoAsync(int categoryId)
        {
            var result = await _uow.Categories.AnyAsync(c => c.Id == categoryId);
            if(result)
            {
                var category = await _uow.Categories.GetAsync(v => v.Id == categoryId);
                var categoryUpdateDto = _mapper.Map<CategoryUpdateDto>(category);
                return new DataResult<CategoryUpdateDto>(ResultStatus.Success, data: categoryUpdateDto);
            }
            else
                return new DataResult<CategoryUpdateDto>(ResultStatus.Success,message:Messages.Category.NotFound(false), data: null);
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var categoriesCount = await _uow.Categories.CountAsync();
            if(categoriesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, categoriesCount);
            }
            return new DataResult<int>(ResultStatus.Error, message: Messages.Category.NotFound(true), -1);
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var categoriesCount = await _uow.Categories.CountAsync(v=> !v.IsDeleted);
            if (categoriesCount > -1)
            {
                return new DataResult<int>(ResultStatus.Success, categoriesCount);
            }
            return new DataResult<int>(ResultStatus.Error, message: Messages.Category.NotFound(true), -1);
        }

        public async Task<IDataResult<CategoryListDto>> GetAllByDeletedAsync()
        {
            var categories = await _uow.Categories.GetAllAsync(c => c.IsDeleted);

            if (categories.Count > -1)
            {
                return new DataResult<CategoryListDto>(ResultStatus.Success, new CategoryListDto
                {
                    Categories = categories,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<CategoryListDto>(ResultStatus.Error, message: Messages.Category.NotFound(true), data: null);
        }

        public async Task<IDataResult<CategoryDto>> UndoDeleteAsync(int categoryId, string modifiedByName)
        {
            var category = await _uow.Categories.GetAsync(v => v.Id == categoryId);
            if (category != null)
            {
                category.IsDeleted = false;
                category.IsActive = true;
                category.ModifiedByName = modifiedByName;
                category.ModifiedDate = DateTime.Now;

                var deletedCategory = await _uow.Categories.UpdateAsync(category);

                if (await _uow.SaveAsync() > 0)
                {
                    return new DataResult<CategoryDto>(ResultStatus.Success, message: Messages.Category.UndoDelete(deletedCategory.Name), data: new CategoryDto
                    {
                        Category = deletedCategory,
                        ResultStatus = ResultStatus.Success,
                        Message = Messages.Category.UndoDelete(deletedCategory.Name)
                    });
                }
                return new DataResult<CategoryDto>(ResultStatus.Error, message: $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz", data: new CategoryDto
                {
                    Category = deletedCategory,
                    ResultStatus = ResultStatus.Error,
                    Message = $"Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz"
                });
            }

            return new DataResult<CategoryDto>(ResultStatus.Error, message: Messages.Category.NotFound(false), data: new CategoryDto
            {
                Category = null,
                ResultStatus = ResultStatus.Error,
                Message = Messages.Category.NotFound(false)
            });
        }
    }
}
