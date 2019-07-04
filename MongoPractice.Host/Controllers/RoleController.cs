using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoPractice.Host.Models;

namespace MongoPractice.Host.Controllers
{
    /// <summary>
    /// 角色管理控制器
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController]
    public class RoleController : ControllerBase
    {
        #region 服务注入
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<ApplicationRole> _roleManager;

        public RoleController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion


        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> POST(string roleName)
        {
            var result = await _roleManager.CreateAsync(new ApplicationRole
            {
                Name = roleName
            });
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return Ok("角色添加成功！");
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = _roleManager.Roles.Select(x => new
            {
                x.Id,
                x.Name,
            }).ToList();
            return new JsonResult(roles);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put(string id, string roleName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                role.Name = roleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return Ok("修改成功！");
                }
                return NotFound("修改时出错了！");
            }
            return NotFound("角色不存在！");
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Ok("删除成功！");
                }
                return NotFound("删除时出错了！");
            }
            return NotFound("角色不存在！");
        }

        /// <summary>
        /// 添加用户到某个角色
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        [HttpPost(nameof(UserToRole))]
        public async Task<ActionResult> UserToRole(string uid, string roleId)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var role = await _roleManager.FindByIdAsync(roleId);
            if (user != null && role != null)
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name);
                if (result.Succeeded)
                {
                    return Ok("添加成功！");
                }
            }
            return NotFound("用户或角色未找到！");
        }
    }
}