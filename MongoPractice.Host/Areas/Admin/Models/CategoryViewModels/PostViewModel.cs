using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace MongoPractice.Host.Areas.Admin.Models.CategoryViewModels
{
    public class PostViewModel
    {
        [Required, StringLength(10)]
        public string Name { get; set; }

        [Required, StringLength(200)]
        public string Details { get; set; }
    }
}
