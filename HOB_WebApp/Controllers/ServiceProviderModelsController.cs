using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;

namespace HOB_WebApp.Models
{
    public class ServiceProviderModelsController : Controller
    {
        private readonly HOB_WebAppContext _context;

        public ServiceProviderModelsController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: ServiceProviderModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceProviderModel.ToListAsync());
        }

        // GET: ServiceProviderModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceProviderModel = await _context.ServiceProviderModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (serviceProviderModel == null)
            {
                return NotFound();
            }

            return View(serviceProviderModel);
        }

        // GET: ServiceProviderModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceProviderModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,service,phone_number")] ServiceProviderModel serviceProviderModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceProviderModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceProviderModel);
        }

        // GET: ServiceProviderModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceProviderModel = await _context.ServiceProviderModel.FindAsync(id);
            if (serviceProviderModel == null)
            {
                return NotFound();
            }
            return View(serviceProviderModel);
        }

        // POST: ServiceProviderModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,service,phone_number")] ServiceProviderModel serviceProviderModel)
        {
            if (id != serviceProviderModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceProviderModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceProviderModelExists(serviceProviderModel.id))
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
            return View(serviceProviderModel);
        }

        // GET: ServiceProviderModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceProviderModel = await _context.ServiceProviderModel
                .FirstOrDefaultAsync(m => m.id == id);
            if (serviceProviderModel == null)
            {
                return NotFound();
            }

            return View(serviceProviderModel);
        }

        // POST: ServiceProviderModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceProviderModel = await _context.ServiceProviderModel.FindAsync(id);
            _context.ServiceProviderModel.Remove(serviceProviderModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceProviderModelExists(int id)
        {
            return _context.ServiceProviderModel.Any(e => e.id == id);
        }
    }
}
