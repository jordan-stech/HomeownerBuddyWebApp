using System;
using System.ComponentModel.DataAnnotations;

namespace HOB_WebApp.Models
{
    public class ServiceProviderModel
    {
        //The automattically generated user ID
        public int id { get; set; }
        //The name of teh service provider
        [Required]
        public string name{ get; set; }
        //What they do
        public string service { get; set; }
        //The phone number for the service
        [Required]
        public string phone_number { get; set; }
        //The url for their website
        public string url { get; set; }
    }
}
