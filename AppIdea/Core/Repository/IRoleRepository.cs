using AppIdea.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace AppIdea.Core.Repository
{
    public interface IRoleRepository
    {
        ICollection<IdentityRole> GetRoles();
       

    }
}
