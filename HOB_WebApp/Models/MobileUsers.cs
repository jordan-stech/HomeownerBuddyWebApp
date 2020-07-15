using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOB_WebApp.Models
{
    public class MobileUsers
    {
        //The ID automattically generated for the mobile user
        public int Id { get; set; }
        //The first name for the user
        public string FName { get; set; }
        //The last name for the user
        public string Lname { get; set; }
        //The homecode linked to the user
        public string Code { get; set; }
        //The address of the homecode
        public string address { get; set; }
        //The date that the user registered
        public string date { get; set; }
        public string InstanceId { get; set; }
    }
}