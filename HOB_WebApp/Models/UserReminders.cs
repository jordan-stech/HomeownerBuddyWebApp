using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOB_WebApp.Models
{
    public class UserReminders
    {
        // Auto generated Id for the plan
        public int Id { get; set; }
        // Maintenance Reminder Id not related to any specific user
        public int ReminderId { get; set; }
        // The Mobile User Id
        public int UserId { get; set; }
        // The Mobile User first name
        public string FName { get; set; }
        // The Mobile User last name
        public string LName { get; set; }
        // Check to see if a homeowner completed a maintenance task
        public string Completed { get; set; }
        // The title of the Maintenance Reminder
        public string Reminder { get; set; }
    }
}
