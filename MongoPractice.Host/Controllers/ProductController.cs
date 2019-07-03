using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoPractice.Host.Services;

namespace MongoPractice.Host.Controllers
{
    /// <summary>
    /// 产品控制器
    /// </summary>
    [Route("api/[controller]")]
    //[ApiController]
    public class ProductController : ControllerBase
    {
        public ProductService ProductService { get; }
        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public ProductController(ProductService productService, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            ProductService = productService;
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns>所有实体</returns>
        [HttpGet]
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
            });
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="productName">名称</param>
        /// <param name="details">详情</param>
        /// <param name="img">图片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(string productName, string details, IFormFile img)
        {
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(img.FileName);
            var filePath = Path.Combine(HostingEnvironment.WebRootPath, fileName);
            if (img.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
            }
            await ProductService.Create(new Models.Product
            {
                ProductDetails = details,
                ProductName = productName,
                ImgUrl = fileName,
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
        public async Task<bool> Put(string id, string productName, string details)
        {
            var entiy = await ProductService.Get(id);
            entiy.ProductDetails = details;
            entiy.ProductName = productName;
            var result = await ProductService.Update(id, entiy);
            return result;
        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id">需要删除的实体ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await ProductService.Remove(id);
        }
    }
}