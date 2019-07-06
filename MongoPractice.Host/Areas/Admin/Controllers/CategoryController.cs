using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoPractice.Host.Areas.Admin.Models.CategoryViewModels;
using MongoPractice.Host.Models;
using MongoPractice.Host.Services;
using MongoPractice.Host.Tool;

namespace MongoPractice.Host.Areas.Admin.Controllers
{
    /// <summary>
    /// 分类控制器
    /// 需要管理员权限
    /// </summary>
    //[ApiController]
    //[Authorize(Roles = "管理员,Admin")]
    [Route("api/Admin/[controller]"), Authorize(Policy = "管理员"), ApiExplorerSettings(GroupName = "admin")]
    public class CategoryController : ControllerBase
    {
        readonly CategoryService _categoryService;
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// 获取所有分类，需要管理员权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var result = await _categoryService.Get();
            return new JsonResult(result.Select(x => new
            {
                x.Name,
                x.Details,
                x.Id
            }));
        }

        /// <summary>
        /// 根据ID获取，需要管理员权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<JsonResult> Get(string id)
        {
            var result = await _categoryService.Get(id);
            return new JsonResult(new
            {
                result.Name,
                result.Details,
                result.Id
            });
        }

        /// <summary>
        /// 添加，需要管理员权限
        /// </summary>
        /// <param name="postViewModel">添加的实体</param>
        [HttpPost]
        public async Task<ActionResult> Post(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var entiy = ModelHelper.EntityMerge(postViewModel, new Category());
                await _categoryService.Create(entiy);
                return Ok("分类添加成功！");
            }
            return BadRequest();
        }

        /// <summary>
        /// 修改，需要管理员权限
        /// </summary>
        /// <param name="id">类别Id</param>
        /// <param name="name">类别名称</param>
        /// <param name="details">详情</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, string name, string details)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(details))
            {
                return BadRequest("请输入修改的名称或者详情");
            }
            var entiy = await _categoryService.Get(id);
            if (entiy == null)
            {
                return NotFound("找不到该记录！");
            }
            if (!string.IsNullOrEmpty(name))
            {
                entiy.Name = name;
            }
            if (!string.IsNullOrEmpty(details))
            {
                entiy.Details = details;
            }
            var updateResult = await _categoryService.Update(id, entiy);
            return Ok(updateResult);
            //return Unauthorized();
        }

        /// <summary>
        /// 删除，需要管理员权限
        /// </summary>
        /// <param name="id">需要删除的分类ID</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var result = await _categoryService.Remove(id);
            return Ok(result);
        }

    }
}