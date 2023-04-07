
using AppIdea.Areas.Identity.Data;
using AppIdea.Core.Repository;
using AppIdea.Core.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;

namespace AppIdea.Controllers
{
    public class UserController : Controller
    {
        
    
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppIdeaUser> _signInManager;
        private readonly AppIdeaContext _context;


        public UserController(IUnitOfWork unitOfWork, SignInManager<AppIdeaUser> signInManager, AppIdeaContext context )
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _context = context;
            
        }


        public IActionResult Index(int page =1)
        {
            var user = _unitOfWork.User.GetUsers(page);
            return View(user);
        }

        public IActionResult Delete(string? id)
        {
            ///get id from view

          

            if (id == null || _unitOfWork.User == null)
            {
                return NotFound();
            }

            var user = _unitOfWork.User.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        ///handel delete user
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {

            if (_unitOfWork.User == null)
            {
                return Problem("Entity set 'AppIdeaContext.AppIdeaUser'  is null.");
            }

            var user = _unitOfWork.User.GetUser(id);

            _unitOfWork.User.DeleteUser(user);
            
            return RedirectToAction(nameof(Index));

        }
        //private bool UserExists(string id)
        //{
        //    return _context.Users.Any(u => u.Id == id);
        //}

        // get uer and role to handle edit
        public async Task<IActionResult> Edit(string id)
        {
            // get id user
            var user = _unitOfWork.User.GetUser(id);

            // get a list of role 
            var roles = _unitOfWork.Role.GetRoles();

            var userRole = await _signInManager.UserManager.GetRolesAsync(user);

            //var roleItem = new List<SelectListItem>();
            //foreach(var role in roles)
            //{
            //    var hasRole = userRole.Any(ur => ur.Contains(role.Name));
            //    roleItem.Add(new SelectListItem(role.Name, role.Id, hasRole));
            //}

            var roleItemSelect = roles.Select(role => 
            new SelectListItem(role.Name, role.Id, userRole.Any(ur =>ur.Contains(role.Name)))).ToList();

            var vm = new EditViewModel
            {
                User = user,
                Role = roleItemSelect
            };
            return View(vm);
        }
        // handle update user and role
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmedAsync(EditViewModel data)
        {
            var user = _unitOfWork.User.GetUser(data.User.Id);

            if(user == null)
            {
                return NotFound();
            }

            var userRoleInDB = await _signInManager.UserManager.GetRolesAsync(user);

            //loop through role in view model
            //checked role was asigned in DB
               //if assigned ---DO nothing
               //if not asigned -- add role

            foreach(var role in data.Role)
            {
                var assignedInDB = userRoleInDB.FirstOrDefault(ur => ur == role.Text);

                if (role.Selected)
                {
                    if(assignedInDB == null)
                    {
                        //add
                        await _signInManager.UserManager.AddToRoleAsync(user, role.Text);
                    }

                }
                else
                {
                    if(assignedInDB != null)
                    {
                        //remove 
                        await _signInManager.UserManager.RemoveFromRoleAsync(user, role.Text);

                    }
                }

            }
            user.Firstname = data.User.Firstname;
            user.Email = data.User.Email;

            _unitOfWork.User.UpdateUser(user);


            return RedirectToAction("Index", new {id = user.Id} );
        }

    
       

}
}
