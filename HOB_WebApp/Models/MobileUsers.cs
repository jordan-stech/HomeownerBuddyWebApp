using System;
using System.ComponentModel.DataAnnotations;

namespace HOB_WebApp.Models
{
    public class MobileUsers
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string Lname { get; set; }
        public string HomeCode { get; set; }
    }
}