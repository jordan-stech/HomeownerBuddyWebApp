using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using HOB_WebApp.Models;

namespace HOB_WebApp.Controllers
{
    public class HomeCodesController : Controller
    {
        private readonly HOB_WebAppContext _context;

        public HomeCodesController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: HomeCodes
        public async Task<IActionResult> Index()
        {
            return View(await _context.HomeCodes.ToListAsync());
        }

        // GET: HomeCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homeCodes = await _context.HomeCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homeCodes == null)
            {
                return NotFound();
            }

            return View(homeCodes);
        }

        // GET: HomeCodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomeCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Address")] HomeCodes homeCodes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(homeCodes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(homeCodes);
        }

        // GET: HomeCodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homeCodes = await _context.HomeCodes.FindAsync(id);
            if (homeCodes == null)
            {
                return NotFound();
            }
            return View(homeCodes);
        }

        // POST: HomeCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Address")] HomeCodes homeCodes)
        {
            if (id != homeCodes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(homeCodes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomeCodesExists(homeCodes.Id))
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
            return View(homeCodes);
        }

        // GET: HomeCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homeCodes = await _context.HomeCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homeCodes == null)
            {
                return NotFound();
            }

            return View(homeCodes);
        }

        // POST: HomeCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var homeCodes = await _context.HomeCodes.FindAsync(id);
            _context.HomeCodes.Remove(homeCodes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomeCodesExists(int id)
        {
            return _context.HomeCodes.Any(e => e.Id == id);
        }
    }
}
