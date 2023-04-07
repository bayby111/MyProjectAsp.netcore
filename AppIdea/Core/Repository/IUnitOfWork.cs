namespace AppIdea.Core.Repository
{
    public interface IUnitOfWork
    {
        public IUserRepository User {get;}
        public IRoleRepository Role { get; }


    }
}
