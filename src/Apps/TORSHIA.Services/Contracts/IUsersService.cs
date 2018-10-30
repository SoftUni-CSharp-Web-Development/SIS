using TORSHIA.App.Models.Binding;
using TORSHIA.Domain;

namespace TORSHIA.Services.Contracts
{
    public interface IUsersService
    {
        void CreateUser(RegisterUserBindingModel registerUserBindingModel);

        User GetUserByUsername(string username);
    }
}
