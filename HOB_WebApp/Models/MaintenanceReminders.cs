using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HOB_WebApp.Models
{
    public class MaintenanceReminders
    {
        //Auto generated ID for the plan
        public int Id { get; set; }

        //The title of the maintenance reminder task
        //[Required]
        public string Reminder { get; set; }
        //The category which the maintenance task belongs to
        //[Required]
        public string Sent { get; set; }
        //Check to see if a homeowner completed a maintenance task
        public string Completed { get; set; }
    }
}
