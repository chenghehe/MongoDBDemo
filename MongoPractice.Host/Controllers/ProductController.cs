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
using MongoPractice.Host.Models;
using MongoPractice.Host.Services;

namespace MongoPractice.Host.Controllers
{
    /// <summary>
    /// 产品控制器
    /// </summary>
    [Route("api/[controller]"), Authorize, ApiExplorerSettings(GroupName = "v1")]
    //[ApiController]
    public class ProductController : ControllerBase
    {
        #region ctor
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(
            ProductService productService,
            CategoryService categoryService,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _categoryService = categoryService;
            _productService = productService;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }
        #endregion

        /// <summary>
        /// 获取所有
        /// 不需要身份验证
        /// </summary>
        /// <returns>所有实体</returns>
        [HttpGet]
        [AllowAnonymous]
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
        /// 不需要身份验证
        /// </summary>
        /// <param name="id"></param>
        /// <returns>单个实体</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
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
                entiy.UserName,
                entiy.Fabulous,
                FabulousCount = entiy.Fabulous.Count,
            });
        }

        /// <summary>
        /// 获取自己发布的
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(GetMyProduct))]
        public async Task<ActionResult<IEnumerable<string>>> GetMyProduct()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var entiys = await _productService.Get(x => x.UserId == user.Id.ToString());
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
        /// 添加
        /// </summary>
        /// <param name="id">类别Id</param>
        /// <param name="productName">文章标题</param>
        /// <param name="details">文章内容</param>
        /// <param name="img">文章图片（目前是单张图）</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([Required]string id, string productName, string details, IFormFile img)
        {
            try
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
                    UserName = user.UserName,
                    UserId = user.Id.ToString(),
                    CategoryId = category.Id,
                });
                return Ok("添加成功");
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
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
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user.Id.ToString() == entiy.UserId)
            {
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
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (result || user.Id.ToString() == entiy.UserId)
            {
                return Ok(await _productService.Remove(entiy.Id));
            }
            return Unauthorized();
        }

        /// <summary>
        /// 评论文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <param name="formId">回复评论的ID（为空则为评论，不为空则为回复评论）</param>
        /// <param name="content">评论内容</param>
        /// <returns></returns>
        [HttpPost(nameof(Comment))]
        public async Task<IActionResult> Comment([Required]string id,
            string formId,
            [Required, StringLength(100, ErrorMessage = "评论内容不能超过一百字")]string content)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("请选择评论的文章以及输入评论的内容！");
            }
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized("请先登录！");
            }
            entiy.Comments.Add(new Comment
            {
                Content = content,
                UserName = user.UserName,
                UserId = user.Id.ToString(),
                FormId = entiy.Comments.Any(x => x.Id == formId) ? formId : default(string),
            });
            var updateResult = await _productService.Update(id, entiy);
            return Ok(updateResult);
        }

        /// <summary>
        /// 点赞文章/取消点赞，点一次是赞，赞了再点就是取消
        /// </summary>
        /// <param name="id">文章Id</param>
        /// <returns></returns>
        [HttpPost(nameof(Fabulous))]
        public async Task<IActionResult> Fabulous([Required]string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("请选择您要点赞的文章！");
            }
            var entiy = await _productService.Get(id);
            if (entiy == null)
            {
                return BadRequest("请选择您要点赞的文章！");
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized("请先登录！");
            }
            if (entiy.Fabulous.Contains(user.UserName))
                entiy.Fabulous.Remove(user.UserName);
            else
                entiy.Fabulous.Add(user.UserName);
            var updateResult = await _productService.Update(id, entiy);
            return Ok(updateResult);
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
                x.UserName,
                x.CreateTime,
                x.Content,
                x.FormId,
                x.Id,
            }));
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(GetCategory))]
        public async Task<ActionResult> GetCategory()
        {
            var result = await _categoryService.Get();
            return new JsonResult(result.Select(x => new
            {
                x.Name,
                x.Details,
                x.Id
            }));
        }



    }
}