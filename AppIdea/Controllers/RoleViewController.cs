using AppIdea.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AppIdea.Controllers
{
    public class RoleViewController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleViewController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index( int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pagesize = 6;
            var role = await _roleManager.Roles.ToPagedListAsync(page, pagesize);
            return View(role);
        }


        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppIdeaRole rolemModel)
        {
         
            if (ModelState.IsValid)
            {
                var role = from r in _roleManager.Roles select r;
                if (role.Any(r => r.Id == rolemModel.Id))
                {
                    
                    ModelState.AddModelError(string.Empty, "Id Existed");
                    return View();
                }

                IdentityRole identityRole = new IdentityRole
                    {

                        Id = rolemModel.Id,
                        Name = rolemModel.Name
                    };

                    IdentityResult result = await _roleManager.CreateAsync(identityRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Roleview");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                

               


            }
            return View(rolemModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {


            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            /// create a new object model and insert  data into object

            var model = new AppIdeaRole
            {
                Id = role.Id,
                Name = role.Name
            };

            // return model value
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditConfirmed(AppIdeaRole roleModel)
        {
            var role = await _roleManager.FindByIdAsync(roleModel.Id);

            if (role == null)
            {
                return NotFound();
            }
            else
            {
               
                role.Name = roleModel.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Roleview");
                }
            }
            return View(roleModel);

        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var model = new AppIdeaRole
            {
                Id = role.Id,
                Name = role.Name
            };

            if(role == null)
            {
                return NotFound();
            }
            

           
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(AppIdeaRole roleModel)
        {
            var role = await _roleManager.FindByIdAsync(roleModel.Id);

            if(role == null)
            {
                return NotFound();
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Roleview");
                }

            }
            return View(roleModel);

        }

    }
}
