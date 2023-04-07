using AppIdea.Areas.Identity.Data;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppIdea.Controllers
{
    public class StaticController : Controller
    {
        private readonly AppIdeaContext _context;

        public StaticController(AppIdeaContext context)
        {
            _context = context;
        }

        public IActionResult ShowData()
        {
            var reactIn = _context.Reacts.Include(r => r.Ideas).ToList();


            return View(reactIn);
        }



        public IActionResult GetData()
        {
            var reactIn = _context.Reacts.Include(r=>r.Ideas).OrderBy(i=>i.Id).Select(i=>i.Ideas.Content).ToList();

            var React = _context.Reacts.OrderBy(r=>r.Id).Select(r => r.Like).ToList();

            List<object> data = new List<object>
            {
                reactIn,
                React
            };



            return Json(data);

        }

    }
}
