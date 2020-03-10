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


        /**
         * This is what we call to return a JSON of every Action Plan in the DB
         **/
        // GET: api/ActionPlanAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentModel>>> GetContentModel()
        {
            return await _context.ContentModel.ToListAsync();
        }


        /**
         * This is what we call to return a specific Action Plan 
         * The "5" that they use in the sample url below is the id of a specific action plan in the db
         * In order for this to work, you must know the particular action plan ID ahead of time
         **/
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

        /**
         * This is a PUT request. I do not think we will need it, but I will leave it in just in case. 
         * The "5" that they use in the sample url below is the id of a specific action plan in the db
         * In order for this to work, you must know the particular action plan ID ahead of time
         **/
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


        /**
         * This is a POST request. I do not think we will need it, but I will leave it in just in case. 
         * The "5" that they use in the sample url below is the id of a specific action plan in the db
         * In order for this to work, you must know the particular action plan ID ahead of time
         **/
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

        /**
         * This is a DELETE request. I do not think we will need it, but I will leave it in just in case. 
         * The "5" that they use in the sample url below is the id of a specific action plan in the db
         * In order for this to work, you must know the particular action plan ID ahead of time
         **/
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
