using Coravel.Invocable;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HOB_WebApp.Controllers
{
    public class UpdateReminderStatusInBackground : IInvocable
    {
        private readonly HOB_WebAppContext _context;

        // Each param injected from the service container ;)

        public UpdateReminderStatusInBackground()
        {
            
        }

        public async Task Invoke()
        {


            /*MaintenanceReminders reminders = new MaintenanceReminders();
                    DateTime currDate = DateTime.Now;

            reminders.ReminderItem = "1";
            reminders.Reminder = "2";
            reminders.NotificationInterval = "Weekly";
            reminders.SeasonFall = "false";
            reminders.SeasonSpring = "false";
            reminders.SeasonSummer = "false";
            reminders.SeasonWinter = "false";
            

            reminders.Description = currDate.ToString();
            _context.MaintenanceReminders.Add(reminders);
            await _context.SaveChangesAsync();*/
                    
                

             
        }
    }
}
