using System.Linq;
using Microsoft.EntityFrameworkCore;
using TORSHIA.App.Models.Binding;
using TORSHIA.Data;
using TORSHIA.Domain;
using TORSHIA.Services.Contracts;

namespace TORSHIA.Services
{
    public class UsersService : IUsersService
    {
        private readonly TorshiaDbContext context;

        public UsersService(TorshiaDbContext context)
        {
            this.context = context;
        }

        public void CreateUser(RegisterUserBindingModel registerUserBindingModel)
        {
            User user = new User
            {
                Username = registerUserBindingModel.Username,
                Email = registerUserBindingModel.Email,
                Password = registerUserBindingModel.Password,
                Role =
                    (this.context.Users.Any()
                        ? this.context.UserRoles.SingleOrDefault(ur => ur.Name == "User")
                        : this.context.UserRoles.SingleOrDefault(ur => ur.Name == "Admin")),
                IsValid = true
            };

            this.context.Users.Add(user);
            this.context.SaveChanges();
        }

        public User GetUserByUsername(string username) 
            => this.context.Users
                .Include(u => u.Role)
                .SingleOrDefault(u => u.Username == username);
    }
}
