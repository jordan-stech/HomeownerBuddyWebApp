using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [AllowAnonymous]
    public class BackgroundAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        public BackgroundAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET api/<BackgroundAPIController>/5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReminders>>> GetFirebaseNotifications()
        {

            var userList = await _context.MobileUsers.ToListAsync();
            var reminderList = await _context.UserReminders.ToListAsync();

            
            FireBasePush push = new FireBasePush("AAAAUZUJsVw:APA91bGGHh_Zfzb4Ry3ywy68mcnuqWMdmFFa1YyoYc4EhCiNPiY95KhR-KAHnFbuE55Az3jiaMO-zLHkQK87UFWPyb_sYwv2o5-uR5YVcn71P1J2lB9aeObdeEkpi5ylaX7awaU4ZYvA");

            // Create a Firebase notification for each user's reminders
            foreach (MobileUsers user in userList)
            {
                int countTodo = 0;
                int countOverdue = 0;
                string userinstanceid = user.InstanceId;

                foreach (UserReminders userReminder in reminderList)
                {                   
                    if (user.Id == userReminder.UserId && userReminder.Completed == "Due")
                    {
                        countTodo++;
                    }

                    else if (user.Id == userReminder.UserId && userReminder.Completed == "Overdue")
                    {
                        countOverdue++;
                    }

                }

                // Create Firebase notification here using the count variables
                push.SendPush(new PushMessage()
                {
                    to = userinstanceid,
                    notification = new PushMessageData
                    {
                        title = "You have new maintenance reminders",
                        text = "You have " + countOverdue + " overdue tasks and " + countTodo + " due tasks.",
                        //click_action = click_action
                    }
                });
            }

            return NoContent();
        }

        // POST api/<BackgroundAPIController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BackgroundAPIController>/5
        [HttpPut("{UserId}")]
        public async Task<ActionResult<UserReminders>> PutAsync(string id)
        {
            var allUserReminders = await _context.UserReminders.ToListAsync();

            // Gets the first day of the next week, which is Sunday
            var firstdayOfNextWeek = DateTime.Now.LastDayOfWeek();

            // Gets the first day of the next month
            var firstdayOfNextMonth = DateTime.Now.FirstDayOfNextMonth();

            // Gets the first day of next year
            var newYear = DateTime.Now.AddYears(1);
            var nextYear = newYear.Year;
            var firstDayOfNextYear = new DateTime(nextYear, 1, 1);

            // Gets the last day of the current year
            var lastDayOfYear = new DateTime(DateTime.Now.Year, 12, 31);

            // First day of last year's winter
            var tempWinter1 = new DateTime(DateTime.Now.Year, 12, 1);
            var tempWinter2 = tempWinter1.AddYears(-1);
            int lastWinter = tempWinter2.Year;

            // Season date ranges
            var startSpring = new DateTime(DateTime.Now.Year, 3, 1);
            var endSpring = new DateTime(DateTime.Now.Year, 5, 31);
            var startSummer = new DateTime(DateTime.Now.Year, 6, 1);
            var endSummer = new DateTime(DateTime.Now.Year, 8, 31);
            var startFall = new DateTime(DateTime.Now.Year, 9, 1);
            var endFall = new DateTime(DateTime.Now.Year, 11, 30);
            var thisYearStartWinter = new DateTime(DateTime.Now.Year, 12, 1);
            var lastYearStartWinter = new DateTime(lastWinter, 12, 1);
            var nextYearEndWinter = new DateTime(nextYear, 2, 28);
            var thisYearEndWinter = new DateTime(DateTime.Now.Year, 2, 28);

            foreach (UserReminders userReminder in allUserReminders)
            {
                // Convert the user's registration date to DateTime object to use the AddDays() function based on the NotificationInterval
                var date = DateTime.Today;
                var newDate = date;

                // Will be used if a reminder is not currently in season
                var noDueDate = "";

                // Check if a user's reminder task is overdue
                if (userReminder.Scheduled == "true" && userReminder.Completed != "Overdue")
                {
                    var dueDate = userReminder.DueDate;
                    int dateDiff = DateTime.Compare(dueDate, date);

                    // If the current date is the same or later than the due date, mark the user's task as Overdue
                    if (dateDiff < 0 || dateDiff == 0)
                    {
                        userReminder.Completed = "Overdue";
                        _context.UserReminders.Update(userReminder);
                        await _context.SaveChangesAsync();
                    }
                }

                var nextStartDate = userReminder.NextStartDate;
                int newDateDiff = DateTime.Compare(nextStartDate, date);

                // The reminder is waiting for a new due date, so check against the current date to assign it 
                if (newDateDiff < 0 || newDateDiff == 0)
                {
                    if (userReminder.Scheduled == "false")
                    {
                        if (((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false")) || ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true")))
                        {
                            if (userReminder.NotificationInterval == "Weekly")
                            {
                                newDate = userReminder.NextStartDate.AddDays(7);
                            }
                            else if (userReminder.NotificationInterval == "Biweekly")
                            {
                                newDate = userReminder.NextStartDate.AddDays(14);
                            }
                            else if (userReminder.NotificationInterval == "Monthly")
                            {
                                newDate = userReminder.NextStartDate.AddDays(30);
                            }
                            else if (userReminder.NotificationInterval == "Quarterly")
                            {
                                newDate = userReminder.NextStartDate.AddDays(90);
                            }
                            else if (userReminder.NotificationInterval == "Yearly")
                            {
                                newDate = userReminder.NextStartDate.AddDays(365);
                            }
                        }

                        // Spring
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endSpring)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(7) <= endSpring)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(14) <= endSpring)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SpringSummer
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endSummer)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(7) <= endSummer)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(14) <= endSummer)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SpringFall
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(7) <= endSpring) || (userReminder.NextStartDate.AddDays(7) <= endFall))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(14) <= endSpring) || (userReminder.NextStartDate.AddDays(14) <= endFall))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(20) <= endSpring) || (userReminder.NextStartDate.AddDays(20) <= endFall))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SpringWinter
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(7) <= endSpring) || (userReminder.NextStartDate.AddDays(7) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(14) <= endSpring) || (userReminder.NextStartDate.AddDays(14) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(20) <= endSpring) || (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SpringSummerFall
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endFall)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(7) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(14) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SpringFallWinter
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(7) <= endSpring) || (userReminder.NextStartDate.AddDays(7) <= endFall) || (userReminder.NextStartDate.AddDays(7) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(14) <= endSpring) || (userReminder.NextStartDate.AddDays(14) <= endFall) || (userReminder.NextStartDate.AddDays(14) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(20) <= endSpring) || (userReminder.NextStartDate.AddDays(20) <= endFall) || (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // Summer
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSummer && date <= endSummer)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(7) <= endSummer)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(14) <= endSummer)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SummerFall
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSummer && date <= endFall)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(7) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(14) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SummerWinter
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSummer && date <= endSummer) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(7) <= endSummer) || (userReminder.NextStartDate.AddDays(7) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(14) <= endSummer) || (userReminder.NextStartDate.AddDays(14) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(20) <= endSummer) || (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // SummerFallWinter
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSummer && date <= nextYearEndWinter) || (date >= startSummer.AddYears(-1) && date <= thisYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(7) <= nextYearEndWinter) || (userReminder.NextStartDate.AddDays(7) <= thisYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(14) <= nextYearEndWinter) || (userReminder.NextStartDate.AddDays(14) <= thisYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter) || (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // Fall
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startFall && date <= endFall)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(7) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (userReminder.NextStartDate.AddDays(14) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // FallWinter
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startFall && date <= nextYearEndWinter) || (date >= startFall.AddYears(-1) && date <= thisYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(7) <= nextYearEndWinter) || (userReminder.NextStartDate.AddDays(7) <= thisYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(14) <= nextYearEndWinter) || (userReminder.NextStartDate.AddDays(14) <= thisYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter) || userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }
                        // Winter
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(7) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(14) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter) || (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = userReminder.NextStartDate.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (userReminder.NextStartDate.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (userReminder.NextStartDate.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                            }
                            else
                            {
                                noDueDate = "Not currently due";
                            }
                        }


                        if (noDueDate != "Not currently due")
                        {
                            userReminder.Scheduled = "true";
                            userReminder.DueDate = newDate;
                            userReminder.Completed = "Due";
                        }
                        else
                        {
                            // Leave DueDate null since it's not in season 
                            userReminder.Completed = "Not in season";
                            userReminder.Scheduled = "false";
                        }

                        _context.UserReminders.Update(userReminder);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return NoContent();
        }

        // DELETE api/<BackgroundAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
