using System;
using System.ComponentModel.DataAnnotations;

namespace HOB_WebApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set;  }
        [Required]
        public string Password { get; set; }
    }
}
