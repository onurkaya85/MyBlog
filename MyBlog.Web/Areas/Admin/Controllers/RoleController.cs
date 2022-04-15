using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlog.Entities.Concrete;
using MyBlog.Entities.Dtos;
using MyBlog.Shared.Utilities.Extensions;
using MyBlog.Shared.Utilities.Results.ComplexType;
using MyBlog.Web.Areas.Admin.Models;
using MyBlog.Web.Helpers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyBlog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : BaseController
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleController(RoleManager<Role> roleManager,UserManager<User> userManager, IMapper mapper,IImageHelper imageHelper)
            :base(userManager,mapper,imageHelper)
        {
            _roleManager = roleManager;
        }

        [Authorize(Roles ="SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            if(roles.Any())
            {
                return View(new RoleListDto
                {
                    Roles = roles
                });
            }
            return View(new RoleListDto());
        }

        [Authorize(Roles = "SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (roles.Any())
            {
                var rolesJsonResult = JsonSerializer.Serialize(new RoleListDto
                {
                    Roles = roles
                });

                return Json(rolesJsonResult);
            }
            return Json(null);
        }

        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpGet]
        public async Task<IActionResult> Assign(int userId)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(v => v.Id == userId);
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await UserManager.GetRolesAsync(user);

            var userRoleAssignDto = new UserRoleAssignDto
            {
                UserId = user.Id,
                UserName = user.UserName,
            };

            foreach(var role in roles)
            {
                var roleAssignDto = new RoleAssignDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    HasRole = userRoles.Contains(role.Name)
                };

                userRoleAssignDto.RoleAssignDtos.Add(roleAssignDto);
            }

            return PartialView("_RoleAssignPartial", userRoleAssignDto);
        }

        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleAssignDto userRoleAssignDto)
        {
            if(ModelState.IsValid)
            {
                var user = await UserManager.Users.SingleOrDefaultAsync(v => v.Id == userRoleAssignDto.UserId);
                foreach(var roleAssignDto in userRoleAssignDto.RoleAssignDtos)
                {
                    if (roleAssignDto.HasRole)
                        await UserManager.AddToRoleAsync(user, roleAssignDto.RoleName);
                    else
                        await UserManager.RemoveFromRoleAsync(user, roleAssignDto.RoleName);
                }

                await UserManager.UpdateSecurityStampAsync(user);

                var userRoleAssignJsonModel = JsonSerializer.Serialize(new UserRoleAssignAjaxViewModel
                {
                    UserDto = new UserDto
                    {
                        User= user,
                        Message = $"{user.UserName} kullanıcısına rol atama başarıyla gerçekleşti.",
                        ResultStatus = ResultStatus.Success
                    },
                    RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial", userRoleAssignDto)
                });

                return Json(userRoleAssignJsonModel);
            }
            else
            {
                var userRoleAssignJsonErrorModel = JsonSerializer.Serialize(new UserRoleAssignAjaxViewModel
                {
                    RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial", userRoleAssignDto),
                    UserRoleAssignDto = userRoleAssignDto
                });

                return Json(userRoleAssignJsonErrorModel);
            }
        }
    }
}
