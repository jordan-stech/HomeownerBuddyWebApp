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
        public string ReminderDescription { get; set; }
        public string ReminderItem { get; set; }
        // The Mobile User Id
        public int UserId { get; set; }
        // The Mobile User's first name
        public string FName { get; set; }
        // The Mobile User's last name
        public string LName { get; set; }
        // The Mobile User's home address
        public string Address { get; set; }
        public string NotificationInterval { get; set; }
        public string SeasonSpring { get; set; }
        public string SeasonSummer { get; set; }
        public string SeasonFall { get; set; }
        public string SeasonWinter { get; set; }
        public int ActionPlanId { get; set; }
        public string ActionPlanTitle { get; set; }
        public string ActionPlanCategory { get; set; }
        public string ActionPlanLink { get; set; }
        public string ActionPlanSteps { get; set; }
        // Check to see if a homeowner completed a maintenance task
        public string Completed { get; set; }
        // The title of the Maintenance Reminder
        public string Reminder { get; set; }
        public string DueDate { get; set; }
        public string LastCompleted { get; set; }
    }
}
