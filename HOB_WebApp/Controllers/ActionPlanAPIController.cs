using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [AllowAnonymous]
    public class ActionPlanAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        public ActionPlanAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: api/ActionPlanAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentModel>>> GetContentModel()
        {
            return await _context.ContentModel.ToListAsync();
        }

        // GET: api/ActionPlanAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContentModel>> GetContentModel(int id)
        {
            var contentModel = await _context.ContentModel.FindAsync(id);

            if (contentModel == null)
            {
                return NotFound();
            }

            return contentModel;
        }

        // PUT: api/ActionPlanAPI/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContentModel(int id, ContentModel contentModel)
        {
            if (id != contentModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(contentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContentModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ActionPlanAPI
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ContentModel>> PostContentModel(ContentModel contentModel)
        {
            _context.ContentModel.Add(contentModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContentModel", new { id = contentModel.Id }, contentModel);
        }

        // DELETE: api/ActionPlanAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ContentModel>> DeleteContentModel(int id)
        {
            var contentModel = await _context.ContentModel.FindAsync(id);
            if (contentModel == null)
            {
                return NotFound();
            }

            _context.ContentModel.Remove(contentModel);
            await _context.SaveChangesAsync();

            return contentModel;
        }

        private bool ContentModelExists(int id)
        {
            return _context.ContentModel.Any(e => e.Id == id);
        }
    }
}
