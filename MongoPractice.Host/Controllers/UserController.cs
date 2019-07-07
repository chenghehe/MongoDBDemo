using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoPractice.Host.Models;
using MongoPractice.Host.Models.UserViewModels;

namespace MongoPractice.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="changePasswordViewModel"></param>
        /// <returns></returns>
        [HttpPut(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                string username = HttpContext.User.Identity.Name;
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var result = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.Password);
                if (result.Succeeded)
                {
                    //修改成功
                    await _signInManager.SignOutAsync();
                }
                else
                {
                    return Ok("密码修改失败！");
                }
            }
            return NotFound("参数验证不通过！");
        }
    }
}