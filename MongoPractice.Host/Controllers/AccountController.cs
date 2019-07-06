using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoPractice.Host.Models;
using MongoPractice.Host.Models.AccountViewModels;
using MongoPractice.Host.Services;

namespace MongoPractice.Host.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        #region ctor
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        #endregion

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GET()
        {
            var user = _userManager.Users.Select(x => new
            {
                x.Id,
                x.UserName,
                x.Email,
                x.Roles,
            }).ToList();
            return new JsonResult(user);
        }


        //
        // POST: /Account/Login
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="model">登录实体</param>
        /// <returns></returns>
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok("登陆成功！");
                }
                if (result.RequiresTwoFactor)
                {
                    return BadRequest();
                }
                if (result.IsLockedOut)
                {
                    return BadRequest();
                }
                else
                {
                    return BadRequest();
                }
            }

            // If we got this far, something failed, redisplay form
            return Ok(model);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: /Account/Register
        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok("注册成功！");
                }
                return NotFound("注册失败！");
            }
            return BadRequest();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        // POST: /Account/LogOff
        [HttpPost(nameof(LogOff)), Authorize]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Ok("已登出");
        }
    }
}
