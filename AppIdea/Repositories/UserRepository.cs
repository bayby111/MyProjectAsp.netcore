using AppIdea.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AppIdea.Core.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly AppIdeaContext _context;

        public UserRepository(AppIdeaContext context)
        {
            _context = context;
        }

       

        //public ICollection<AppIdeaRole> GetRole()
        //{
        //    throw new NotImplementedException();
        //}

        public IPagedList<AppIdeaUser> GetUsers(int page)
        {

            page = page < 1 ? 1 : page;
            int pagesize = 6;
            var user = _context.Users.Include(u=> u.Departments).ToPagedList( page, pagesize);
            return user;

        }

        public AppIdeaUser GetUser(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public AppIdeaUser UpdateUser(AppIdeaUser user)
        {
            _context.Update(user);
            _context.SaveChanges();
            return user;
        }

        public AppIdeaUser DeleteUser(AppIdeaUser user)
        {
            _context.Remove(user);
            _context.SaveChanges();
            return user;
        }
    }
}
