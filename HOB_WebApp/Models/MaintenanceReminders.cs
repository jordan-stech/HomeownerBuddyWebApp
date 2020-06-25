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
        //The description of the maintenance reminder task
        public string Description { get; set; }
        //When to set the due date for a maintenance reminder task
        public string NotificationInterval { get; set; }
    }
}
