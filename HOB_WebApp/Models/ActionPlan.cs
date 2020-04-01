using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HOB_WebApp.Models
{
    public class ContentModel
    {
        //Auto generated ID for the plan
        public int Id { get; set; }

        //The title of the plan
        [Required]
        public string Title { get; set; }
        //The youtube link to the video displaying the action plan
        [Required]
        public string Link { get; set; }
        //The steps required to complete action plan
        [Required]
        public string Steps { get; set; }
        //The category of matinence the action plan falls under
        [Required]
        public string Category { get; set; }
        //Discriptive words about the action plan
        public string Tags { get; set; }

    }
}
