using System;
using System.ComponentModel.DataAnnotations;

namespace HOB_WebApp.Models
{
    public class ServiceProviderModel
    {
        public int id { get; set; }
        [Required]
        public string name{ get; set; }
        public string service { get; set; }
        [Required]
        public string phone_number { get; set; }

        public string url { get; set; }
    }
}
