using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOB_WebApp.Models
{
    public class MaintenanceReminders
    {
        //Auto generated ID for the plan
        public int Id { get; set; }
        //The title of the maintenance reminder task
        public string Reminder { get; set; }
        //The category which the maintenance task belongs to
        public string Description { get; set; }
        //Check to see if a homeowner completed a maintenance task
        public int NotificationInterval { get; set; }
    }
}
