using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class MaintenanceReminderAPIController : Controller
    {
        private readonly HOB_WebAppContext _context;

        //Get the DB context
        public MaintenanceReminderAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }


        /**
         * This is what we call to return a JSON of every Maintenance Reminder in the DB
         **/
        //When a "GET" request received, return all Maintenance Reminders
        [HttpGet]
        // GET: Issue
        public IActionResult Index()
        {
            return Json(_context.MaintenanceReminders);
        }
    }
}
