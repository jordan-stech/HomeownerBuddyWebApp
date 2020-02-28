using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]

    public class ServiceProviderAPIController : Controller
    {
        private readonly HOB_WebAppContext _context;

        public ServiceProviderAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }
        [HttpGet]
        // GET: Issue
        public IActionResult Index()
        {
            return Json(_context.ServiceProviderModel);
        }
    }
}