using System.Linq;
using IRunesWebApp.Data;
using IRunesWebApp.Services.Contracts;
using Services;

namespace IRunesWebApp.Services
{
    public class UsersService : IUsersService
    {
        private readonly IRunesContext context;
        private readonly IHashService hashService;

        public UsersService(IRunesContext context, IHashService hashService)
        {
            this.context = context;
            this.hashService = hashService;
        }

        public bool ExistsByUsernameAndPassword(string username, string password)
        {
            var hashedPassword = this.hashService.Hash(password);

            var userExists = this.context.Users
                .Any(u => u.Username == username &&
                          u.HashedPassword == hashedPassword);

            return userExists;
        }
    }
}
