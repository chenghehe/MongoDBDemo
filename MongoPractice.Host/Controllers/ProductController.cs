using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoPractice.Host.Models;
using MongoPractice.Host.Services;

namespace MongoPractice.Host.Controllers
{
    /// <summary>
    /// 产品控制器
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        public ProductService ProductService { get; }
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public UserManager<ApplicationUser> UserManager { get; }

        public ProductController(ProductService productService, IConfiguration configuration, IHostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
        {
            ProductService = productService;
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            UserManager = userManager;
        }

        /// <summary>
        /// 获取所有
        /// 不需要身份验证
        /// </summary>
        /// <returns>所有实体</returns>
        [HttpGet]
        [AllowAnonymous]        
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var entiys = await ProductService.Get();
            if (!entiys?.Any() ?? false)
            {
                return NotFound();
            }
            var result = entiys.Select(x => new
            {
                x.ProductName,
                x.ProductDetails,
                ImgUrl = Configuration.GetSection("url").Value + x.ImgUrl,
                x.Id,
                x.UserId
            });
            return new JsonResult(result);
        }

        /// <summary>
        /// 根据id获取
        /// 不需要身份验证
        /// </summary>
        /// <param name="id"></param>
        /// <returns>单个实体</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Get(string id)
        {
            var entiy = await ProductService.Get(id);
            if (entiy == null)
            {
                return NotFound();
            }
            return new JsonResult(new
            {
                entiy.ProductName,
                entiy.ProductDetails,
                ImgUrl = Configuration.GetSection("url").Value + entiy.ImgUrl,
                entiy.Id,
                entiy.UserId
            });
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="productName">名称</param>
        /// <param name="details">详情</param>
        /// <param name="img">图片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(string productName, string details, IFormFile img)
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized("请先登录！");
            }
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(img.FileName);
            var filePath = Path.Combine(HostingEnvironment.WebRootPath, fileName);
            if (img.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
            }
            await ProductService.Create(new Product
            {
                ProductDetails = details,
                ProductName = productName,
                ImgUrl = fileName,
                UserId = user.Id.ToString(),
            });
            return Ok();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">需要修改的ID</param>
        /// <param name="productName">修改后的名称</param>
        /// <param name="details">修改后的详情</param>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, string productName, string details)
        {
            var result = User.IsInRole(role: "管理员");
            var entiy = await ProductService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            var user = await UserManager.GetUserAsync(HttpContext.User);
            if (result || user.Id.ToString() == entiy.UserId)
            {
                entiy.ProductDetails = details;
                entiy.ProductName = productName;
                var updateResult = await ProductService.Update(id, entiy);
                return Ok(updateResult);
            }
            return Unauthorized();
        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id">需要删除的实体ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = User.IsInRole(role: "管理员");
            var entiy = await ProductService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            var user = await UserManager.GetUserAsync(HttpContext.User);
            if (result || user.Id.ToString() == entiy.UserId)
            {
                return Ok(await ProductService.Remove(entiy.Id));
            }
            return Unauthorized();
        }
    }
}