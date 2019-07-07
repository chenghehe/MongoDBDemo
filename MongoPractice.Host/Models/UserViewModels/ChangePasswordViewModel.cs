using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MongoPractice.Host.Models.UserViewModels
{
    public class ChangePasswordViewModel
    {

        /// <summary>
        /// 原密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "原密码")]
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string Password { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配.")]
        public string ConfirmPassword { get; set; }
    }
}
