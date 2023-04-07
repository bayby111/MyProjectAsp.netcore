using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppIdea.Areas.Identity.Data;
using X.PagedList;

namespace AppIdea.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly AppIdeaContext _context;

        public DepartmentController(AppIdeaContext context)
        {
            _context = context;
        }

        // GET: Department
        public async Task<IActionResult> Index( int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pagesize = 6;
            var depart = await _context.Departments.ToPagedListAsync(page, pagesize);

              return View(depart);
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Department department)
        {

            if (ModelState.IsValid)
            {
                var depart = from d in _context.Departments select d;
                if(depart.Any(d=>d.Id == department.Id))
                {
                    ModelState.AddModelError(string.Empty, "Id Exissted");
                    return View();
                }
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(department);
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            return View(department);
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'AppIdeaContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(string id)
        {
          return _context.Departments.Any(e => e.Id == id);
        }
    }
}
