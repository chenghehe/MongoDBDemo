using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoPractice.Host.Models;
using MongoPractice.Host.Services;

namespace MongoPractice.Host.Areas.Admin.Controllers
{
    //[ApiController]
    [Route("api/Admin/[controller]"), Authorize(Policy = "管理员"), ApiExplorerSettings(GroupName = "admin")]
    public class ProductController : ControllerBase
    {
        readonly CategoryService _categoryService;
        readonly ProductService _productService;
        readonly IConfiguration _configuration;
        readonly IHostingEnvironment _hostingEnvironment;
        readonly UserManager<ApplicationUser> _userManager;
        readonly ILogger _logger;
        public ProductController(
            ProductService productService,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            UserManager<ApplicationUser> userManager,
            ILogger<ProductController> logger,
            CategoryService categoryService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有，所有方法需要管理员权限
        /// </summary>
        /// <returns>所有实体</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var entiys = await _productService.Get();
            //if (!entiys?.Any() ?? false)
            //{
            //    return NotFound();
            //}
            var result = entiys.Select(x => new
            {
                x.ProductName,
                x.ProductDetails,
                ImgUrl = _configuration.GetSection("url").Value + x.ImgUrl,
                x.Id,
                x.UserId,
                x.UserName,
                FabulousCount = x.Fabulous.Count,
            });
            return new JsonResult(result);
        }

        /// <summary>
        /// 根据id获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns>单个实体</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return NotFound();
            }
            return new JsonResult(new
            {
                entiy.ProductName,
                entiy.ProductDetails,
                ImgUrl = _configuration.GetSection("url").Value + entiy.ImgUrl,
                entiy.Id,
                entiy.UserId,
                entiy.Fabulous,
                entiy.UserName,
                FabulousCount = entiy.Fabulous.Count,
            });
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="id">类别Id</param>
        /// <param name="productName">名称</param>
        /// <param name="details">详情</param>
        /// <param name="img">图片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([Required]string id, string productName, string details, IFormFile img)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized("请先登录！");
            }
            var category = await _categoryService.Get(id);
            if (category == null)
            {
                return NotFound("请选择类别！");
            }
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(img.FileName);
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, fileName);
            if (img.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
            }
            await _productService.Create(new Product
            {
                ProductDetails = details,
                ProductName = productName,
                ImgUrl = fileName,
                UserId = user.Id.ToString(),
                CategoryId = category.Id,
                UserName = user.UserName,
            });
            return Ok();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">需要修改的ID</param>
        /// <param name="productName">修改后的名称</param>
        /// <param name="details">修改后的详情</param>
        /// <param name="cId">类别Id</param>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put([Required]string id, string productName, string details, string cId)
        {
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            if (!string.IsNullOrWhiteSpace(productName))
            {
                entiy.ProductName = productName;
            }
            if (!string.IsNullOrWhiteSpace(details))
            {
                entiy.ProductDetails = details;
            }
            if (!string.IsNullOrWhiteSpace(cId))
            {
                var category = await _categoryService.Get(id);
                if (category == null)
                {
                    return NotFound("类别不存在！");
                }
                entiy.CategoryId = cId;
            }
            var updateResult = await _productService.Update(id, entiy);
            return Ok(updateResult);
            //return Unauthorized();
        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id">需要删除的实体ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return Ok(await _productService.Remove(entiy.Id));
            //return Unauthorized();
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id">文章Id</param>
        /// <param name="cId">评论Id</param>
        /// <returns></returns>
        [HttpDelete(nameof(DeleteComment))]
        public async Task<IActionResult> DeleteComment([Required]string id, string cId)
        {
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            try
            {
                var cIndex = entiy.Comments.FindIndex(x => x.Id == cId);
                entiy.Comments.RemoveAt(cIndex);
                var result = await _productService.Update(id, entiy);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return BadRequest("网络异常！");
            }
        }

        /// <summary>
        /// 获取文章的评论内容
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(GetComment)), AllowAnonymous]
        public async Task<ActionResult> GetComment([Required]string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("id错误！");
            }
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return BadRequest("找不到该记录！");
            }
            return new JsonResult(entiy.Comments.Select(x => new
            {
                x.Id,
                x.UserName,
                x.CreateTime,
                x.Content,
            }));
        }
    }
}