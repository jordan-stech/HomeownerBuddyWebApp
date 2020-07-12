using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coravel.Scheduling;
using Coravel.Queuing.Interfaces;
using System.Data.SqlTypes;

namespace HOB_WebApp.Controllers
{
    public class MaintenanceReminderController : Controller
    {
        private readonly HOB_WebAppContext _context;

        public MaintenanceReminderController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: MaintenanceReminders
        public async Task<IActionResult> Index(string searchString)
        {
            return View(await _context.MaintenanceReminders.ToListAsync());
        }

        // GET: UserReminders
        public async Task<IActionResult> Status(string searchString)
        {
            var users = from m in _context.UserReminders
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.LName.Contains(searchString));
                return View(await users.ToListAsync());
            }


            ViewBag.MobileUsers = await _context.MobileUsers.ToListAsync();

            var reminderList = await _context.UserReminders.ToListAsync();


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

            var date = DateTime.Today;
            var newDate = date;

            // Will be used if a reminder is not currently in season
            var noDueDate = "";

            foreach (UserReminders userReminder in reminderList)
            {
                // Check if a user's reminder task is overdue
                if (userReminder.Scheduled == "true" && userReminder.Completed != "Overdue")
                {
                    DateTime currDate = DateTime.Now;
                    DateTime dueDate = userReminder.DueDate;
                    //DateTime lastCompletedDate = Convert.ToDateTime(reminders.LastCompleted);

                    int dateDiff = DateTime.Compare(dueDate, currDate);

                    // If the current date is later than the due date, mark the user's task as Overdue
                    if (dateDiff < 0)
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

                /*int dateDiff = DateTime.Compare(dueDate, lastCompletedDate);
                if ((dateDiff > 0) && (reminders.LastCompleted != null) && (reminders.Completed == "Completed"))
                {
                    reminders.DueDate = "Not currently due";
                    _context.UserReminders.Update(reminders);
                    await _context.SaveChangesAsync();
                }*/
            //}
            return View(await _context.UserReminders.OrderBy(m => m.UserId).ToListAsync());
        }

        // GET: MaintenanceReminders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceReminders = await _context.MaintenanceReminders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceReminders == null)
            {
                return NotFound();
            }

            return View(maintenanceReminders);
        }

        // GET: MaintenanceReminders/Status/Details/5
        public async Task<IActionResult> UserReminderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userReminders = await _context.UserReminders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userReminders == null)
            {
                return NotFound();
            }

            return View(userReminders);
        }

        // GET: MaintenanceReminders/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ActionPlans = await _context.ContentModel.ToListAsync();
            return View();
        }

        // POST: MaintenanceReminders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReminderItem,Reminder,Description,NotificationInterval,ActionPlanId,ActionPlanTitle,ActionPlanCategory,,ActionPlanLink,ActionPlanSteps,SeasonSpring,SeasonSummer,SeasonFall,SeasonWinter")] MaintenanceReminders maintenanceReminders)
        {

            if (ModelState.IsValid)
            {
                // Grab the selected action plan's ID and Category for later use
                var currentAPList = await _context.ContentModel.Where(m => m.Title == maintenanceReminders.ActionPlanTitle).ToListAsync();
                ContentModel currentAP = currentAPList.Find(m => m.Title == maintenanceReminders.ActionPlanTitle);
                if (maintenanceReminders.ActionPlanTitle != "None")
                {
                    maintenanceReminders.ActionPlanId = currentAP.Id;
                    maintenanceReminders.ActionPlanCategory = currentAP.Category;
                    maintenanceReminders.ActionPlanLink = currentAP.Link;
                    maintenanceReminders.ActionPlanSteps = currentAP.Steps;
                }

                _context.Add(maintenanceReminders);
                await _context.SaveChangesAsync();

                var mobileUserList = await _context.MobileUsers.ToListAsync();
                var userReminderList = await _context.UserReminders.ToListAsync();
               

                // After the new minatenance reminder has been added to the db, add an entry for that reminder to each current mobile user on the Reminder Status page
                foreach (MobileUsers mobileUser in mobileUserList)
                {
                    // Convert the user's registration date to DateTime object to use the AddDays() function based on the NotificationInterval
                    var date = DateTime.Today;
                    DateTime newDate = new DateTime();

                    /*var timeUtc = DateTime.UtcNow;
                    var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    var timeEST = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);*/
                    
                    //var newDate = timeEST;
                    //var date = timeEST;

                    UserReminders userReminder = new UserReminders();

                    userReminder.ReminderId = maintenanceReminders.Id;
                    userReminder.ReminderDescription = maintenanceReminders.Description;
                    userReminder.ReminderItem = maintenanceReminders.ReminderItem;
                    userReminder.UserId = mobileUser.Id;
                    userReminder.FName = mobileUser.FName;
                    userReminder.LName = mobileUser.Lname;
                    userReminder.Address = mobileUser.address;
                    userReminder.NotificationInterval = maintenanceReminders.NotificationInterval;
                    userReminder.SeasonSpring = maintenanceReminders.SeasonSpring;
                    userReminder.SeasonSummer = maintenanceReminders.SeasonSummer;
                    userReminder.SeasonFall = maintenanceReminders.SeasonFall;
                    userReminder.SeasonWinter = maintenanceReminders.SeasonWinter;
                    userReminder.ActionPlanId = maintenanceReminders.ActionPlanId;
                    userReminder.ActionPlanTitle = maintenanceReminders.ActionPlanTitle;
                    userReminder.ActionPlanCategory = maintenanceReminders.ActionPlanCategory;
                    userReminder.ActionPlanLink = maintenanceReminders.ActionPlanLink;
                    userReminder.ActionPlanSteps = maintenanceReminders.ActionPlanSteps;
                    userReminder.Reminder = maintenanceReminders.Reminder;
                    userReminder.DueDate = 
                    userReminder.LastCompleted = (DateTime)SqlDateTime.Null;
                    userReminder.PrevDueDate = (DateTime)SqlDateTime.Null;
                    userReminder.NextStartDate = (DateTime)SqlDateTime.Null;




                    // Will be used if a reminder is not currently in season
                    var noDueDate = "";

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

                    /*// Grab the first of the month for the next month
                    DateTime temp2 = DateTime.Today;
                    DateTime month = new DateTime(temp2.AddMonths(1).Year, temp2.AddMonths(1).Month, 1);
                    var nextMonth = month.ToString("MM/dd/yyyy");*/

                    // If season is "All year"
                    if (((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false")) || ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true")))
                    {
                        if (maintenanceReminders.NotificationInterval == "Weekly")
                        {
                            newDate = date.AddDays(7);
                        }
                        else if (maintenanceReminders.NotificationInterval == "Biweekly")
                        {
                            newDate = date.AddDays(14);
                        }
                        else if (maintenanceReminders.NotificationInterval == "Monthly")
                        {
                            newDate = date.AddDays(30);
                        }
                        else if (maintenanceReminders.NotificationInterval == "Quarterly")
                        {
                            newDate = date.AddDays(90);                            
                        }
                        else if (maintenanceReminders.NotificationInterval == "Yearly")
                        {
                            newDate = date.AddDays(365);
                        }
                    }

                    // Spring
                    else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false"))
                    {
                        if (date >= startSpring && date <= endSpring)
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endSpring)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
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
                    else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false"))
                    {
                        if (date >= startSpring && date <= endSummer)
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endSummer)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer) 
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer)
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
                    else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                    {
                        if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= endFall))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= endFall))
                                {
                                    newDate = date.AddDays(14);
                                }                              
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= endFall))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
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
                    else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "true"))
                    {
                        if ((date >= startSpring && date <= endSpring) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
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
                    else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                    {
                        if (date >= startSpring && date <= endFall)
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endFall)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
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
                    else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true"))
                    {
                        if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= endFall) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= endFall) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= endFall) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
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
                    else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false"))
                    {
                        if (date >= startSummer && date <= endSummer)
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endSummer)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
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
                    else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                    {
                        if (date >= startSummer && date <= endFall)
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endFall)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
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
                    else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "true"))
                    {
                        if ((date >= startSummer && date <= endSummer) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSummer) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSummer) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSummer) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
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
                    else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true"))
                    {
                        if ((date >= startSummer && date <= nextYearEndWinter) || (date >= startSummer.AddYears(-1) && date <= thisYearEndWinter))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= nextYearEndWinter) || (date.AddDays(7) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= nextYearEndWinter) || (date.AddDays(14) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= nextYearEndWinter) || (date.AddDays(20) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
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
                    else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                    {
                        if (date >= startFall && date <= endFall)
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endFall)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endFall)
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
                    else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true"))
                    {
                        if ((date >= startFall && date <= nextYearEndWinter) || (date >= startFall.AddYears(-1) && date <= thisYearEndWinter))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= nextYearEndWinter) || (date.AddDays(7) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= nextYearEndWinter) || (date.AddDays(14) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= nextYearEndWinter) || date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
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
                    else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "true"))
                    {
                        if ((date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
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

                        userReminder.PrevDueDate = userReminder.DueDate;
                    }
                    else
                    {
                        // Leave DueDate null since it's not in season 
                        userReminder.Completed = "Not in season";
                        userReminder.Scheduled = "false";
                    }
                    
                    _context.UserReminders.Add(userReminder);
                    await _context.SaveChangesAsync();
                }

                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maintenanceReminders);
        }

        // GET: MaintenanceReminders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.ActionPlans = await _context.ContentModel.ToListAsync();

            var maintenanceReminders = await _context.MaintenanceReminders.FindAsync(id);
            if (maintenanceReminders == null)
            {
                return NotFound();
            }
            return View(maintenanceReminders);
        }

        // POST: MaintenanceReminders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReminderItem,Reminder,Description,NotificationInterval,ActionPlanId,ActionPlanTitle,ActionPlanCategory,,ActionPlanLink,ActionPlanSteps,SeasonSpring,SeasonSummer,SeasonFall,SeasonWinter")] MaintenanceReminders maintenanceReminders)
        {
            if (id != maintenanceReminders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Grab the action plan ID and Category tied to the maintenance reminder to be able to update it
                    var currentAPList = await _context.ContentModel.Where(m => m.Title == maintenanceReminders.ActionPlanTitle).ToListAsync();
                    ContentModel currentAP = currentAPList.Find(m => m.Title == maintenanceReminders.ActionPlanTitle);
                    if (maintenanceReminders.ActionPlanTitle != "None")
                    {
                        maintenanceReminders.ActionPlanId = currentAP.Id;
                        maintenanceReminders.ActionPlanCategory = currentAP.Category;
                        maintenanceReminders.ActionPlanLink = currentAP.Link;
                        maintenanceReminders.ActionPlanSteps = currentAP.Steps;
                    }

                    _context.Update(maintenanceReminders);
                    await _context.SaveChangesAsync();

                    var userReminderList = await _context.UserReminders.Where(m => m.ReminderId == id).ToListAsync();

                    foreach (UserReminders userReminder in userReminderList)
                    {
                        // Convert the user's registration date to DateTime object to use the AddDays() function based on the NotificationInterval
                        var date = DateTime.Today;
                        var newDate = date;

                        userReminder.ReminderItem = maintenanceReminders.ReminderItem;
                        userReminder.Reminder = maintenanceReminders.Reminder;
                        userReminder.ReminderDescription = maintenanceReminders.Description;
                        userReminder.NotificationInterval = maintenanceReminders.NotificationInterval;
                        userReminder.ActionPlanId = maintenanceReminders.ActionPlanId;
                        userReminder.ActionPlanTitle = maintenanceReminders.ActionPlanTitle;
                        userReminder.ActionPlanCategory = maintenanceReminders.ActionPlanCategory;
                        userReminder.ActionPlanLink = maintenanceReminders.ActionPlanLink;
                        userReminder.ActionPlanSteps = maintenanceReminders.ActionPlanSteps;
                        userReminder.SeasonSpring = maintenanceReminders.SeasonSpring;
                        userReminder.SeasonSummer = maintenanceReminders.SeasonSummer;
                        userReminder.SeasonFall = maintenanceReminders.SeasonFall;
                        userReminder.SeasonWinter = maintenanceReminders.SeasonWinter;


                        // Will be used if a reminder is not currently in season
                        var noDueDate = "";

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

                        /*// Grab the first of the month for the next month
                        DateTime temp2 = DateTime.Today;
                        DateTime month = new DateTime(temp2.AddMonths(1).Year, temp2.AddMonths(1).Month, 1);
                        var nextMonth = month.ToString("MM/dd/yyyy");*/

                        // If season is "All year"
                        if (((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false")) || ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true")))
                        {
                            if (maintenanceReminders.NotificationInterval == "Weekly")
                            {
                                newDate = date.AddDays(7);
                            }
                            else if (maintenanceReminders.NotificationInterval == "Biweekly")
                            {
                                newDate = date.AddDays(14);
                            }
                            else if (maintenanceReminders.NotificationInterval == "Monthly")
                            {
                                newDate = date.AddDays(30);
                            }
                            else if (maintenanceReminders.NotificationInterval == "Quarterly")
                            {
                                newDate = date.AddDays(90);
                            }
                            else if (maintenanceReminders.NotificationInterval == "Yearly")
                            {
                                newDate = date.AddDays(365);
                            }
                        }

                        // Spring
                        else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endSpring)
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if (date.AddDays(7) <= endSpring)
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSpring)
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
                        else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endSummer)
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if (date.AddDays(7) <= endSummer)
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endSummer)
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
                        else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall))
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= endFall))
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= endFall))
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= endFall))
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endFall)
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
                        else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "true"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
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
                        else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endFall)
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if (date.AddDays(7) <= endFall)
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= endFall)
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
                        else if ((maintenanceReminders.SeasonSpring == "true") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= endFall) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= endFall) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= endFall) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
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
                        else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "false"))
                        {
                            if (date >= startSummer && date <= endSummer)
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if (date.AddDays(7) <= endSummer)
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSummer)
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
                        else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                        {
                            if (date >= startSummer && date <= endFall)
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if (date.AddDays(7) <= endFall)
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= endFall)
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
                        else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "true"))
                        {
                            if ((date >= startSummer && date <= endSummer) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if ((date.AddDays(7) <= endSummer) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if ((date.AddDays(14) <= endSummer) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if ((date.AddDays(20) <= endSummer) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
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
                        else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "true") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true"))
                        {
                            if ((date >= startSummer && date <= nextYearEndWinter) || (date >= startSummer.AddYears(-1) && date <= thisYearEndWinter))
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if ((date.AddDays(7) <= nextYearEndWinter) || (date.AddDays(7) <= thisYearEndWinter))
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if ((date.AddDays(14) <= nextYearEndWinter) || (date.AddDays(14) <= thisYearEndWinter))
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if ((date.AddDays(20) <= nextYearEndWinter) || (date.AddDays(20) <= thisYearEndWinter))
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
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
                        else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "false"))
                        {
                            if (date >= startFall && date <= endFall)
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if (date.AddDays(7) <= endFall)
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endFall)
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
                        else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "true") && (maintenanceReminders.SeasonWinter == "true"))
                        {
                            if ((date >= startFall && date <= nextYearEndWinter) || (date >= startFall.AddYears(-1) && date <= thisYearEndWinter))
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if ((date.AddDays(7) <= nextYearEndWinter) || (date.AddDays(7) <= thisYearEndWinter))
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if ((date.AddDays(14) <= nextYearEndWinter) || (date.AddDays(14) <= thisYearEndWinter))
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if ((date.AddDays(20) <= nextYearEndWinter) || date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= thisYearEndWinter)
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
                        else if ((maintenanceReminders.SeasonSpring == "false") && (maintenanceReminders.SeasonSummer == "false") && (maintenanceReminders.SeasonFall == "false") && (maintenanceReminders.SeasonWinter == "true"))
                        {
                            if ((date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (maintenanceReminders.NotificationInterval == "Weekly")
                                {
                                    if ((date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Biweekly")
                                {
                                    if ((date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Monthly")
                                {
                                    if ((date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                    {
                                        newDate = date.AddDays(30);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (maintenanceReminders.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(20) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(20) <= nextYearEndWinter)
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
                            userReminder.DueDate = newDate;
                            userReminder.Completed = "Due";
                        }
                        else
                        {
                            // Leave DueDate null since it's not in season 
                            userReminder.Completed = "Not in season";
                        }

                        _context.UserReminders.Update(userReminder);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceRemindersExists(maintenanceReminders.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(maintenanceReminders);
        }

        // GET: MaintenanceReminders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceReminders = await _context.MaintenanceReminders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceReminders == null)
            {
                return NotFound();
            }

            return View(maintenanceReminders);
        }

        // POST: MaintenanceReminders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenanceReminders = await _context.MaintenanceReminders.FindAsync(id);
            var userReminderList = await _context.UserReminders.Where(m => m.ReminderId == id).ToListAsync();

            foreach (UserReminders userReminder in userReminderList)
            {
                _context.UserReminders.Remove(userReminder);
            }

            _context.MaintenanceReminders.Remove(maintenanceReminders);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceRemindersExists(int id)
        {
            return _context.MaintenanceReminders.Any(e => e.Id == id);
        }
    }

    
    }
