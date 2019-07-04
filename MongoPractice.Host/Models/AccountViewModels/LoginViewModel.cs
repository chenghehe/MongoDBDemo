using System.ComponentModel.DataAnnotations;

namespace MongoPractice.Host.Models.AccountViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        ///是否记住
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = true;
    }
}
