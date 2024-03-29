﻿using MyBlog.Entities.Concrete;
using MyBlog.Entities.Dtos;
using MyBlog.Shared.Utilities.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Services.Abstract
{
    public interface ICategoryService
    {
        Task<IDataResult<CategoryDto>> GetAsync(int categoryId);

        Task<IDataResult<CategoryUpdateDto>> GetCategoryUpdateDtoAsync(int categoryId);

        Task<IDataResult<CategoryListDto>> GetAllAsync();

        Task<IDataResult<CategoryListDto>> GetAllByNonDeletedAsync();

        Task<IDataResult<CategoryListDto>> GetAllByDeletedAsync();

        Task<IDataResult<CategoryListDto>> GetAllByNonDeletedAndActiveAsync();

        Task<IDataResult<CategoryDto>> AddAsync(CategoryAddDto categoryAddDto,string createdByName);

        Task<IDataResult<CategoryDto>> UpdateAsync(CategoryUpdateDto categoryUpdateDto, string modifiedByName);

        Task<IDataResult<CategoryDto>> DeleteAsync(int categoryId,string modifiedByName);

        Task<IDataResult<CategoryDto>> UndoDeleteAsync(int categoryId, string modifiedByName);

        Task<IResult> HardDeleteAsync(int categoryId);

        Task<IDataResult<int>> CountAsync();

        Task<IDataResult<int>> CountByNonDeletedAsync();
    }
}
