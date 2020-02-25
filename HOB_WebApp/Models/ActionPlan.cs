using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HOB_WebApp.Models
{
    public class ContentModel
    {
        public int Id { get; set; }

        [Required]
        public string Link { get; set; }
        [Required]
        public string Steps { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
