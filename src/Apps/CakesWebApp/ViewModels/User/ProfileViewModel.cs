using System;

namespace CakesWebApp.ViewModels.User
{
    public class ProfileViewModel
    {
        public string Username { get; set; }

        public DateTime RegisteredOn { get; set; }

        public int OrdersCount { get; set; }
    }
}
