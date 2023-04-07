using AppIdea.Areas.Identity.Data;
using X.PagedList;

namespace AppIdea.Core.Repository
{
    /// <summary>
    // Create interface user adn role 
    /// </summary>
    public interface IUserRepository
    {
        IPagedList<AppIdeaUser> GetUsers(int page);
        AppIdeaUser GetUser(string id);

        AppIdeaUser UpdateUser(AppIdeaUser user);
        AppIdeaUser DeleteUser(AppIdeaUser user);


    }
}
