using AppIdea.Areas.Identity.Data;
using AppIdea.Core.Repository;
using Microsoft.AspNetCore.Identity;

namespace AppIdea.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppIdeaContext _context;

        public RoleRepository(AppIdeaContext context)
        {
            _context = context;
        }

       
        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
