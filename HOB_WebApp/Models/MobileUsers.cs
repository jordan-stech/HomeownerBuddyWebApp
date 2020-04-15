using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOB_WebApp.Models
{
    public class MobileUsers
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string Lname { get; set; }

        public string Code { get; set; }

        public string address { get; set; }
    }
}