using System.Collections.Generic;

namespace IRunesWebApp.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public IEnumerable<NestedViewModel> NestedViewModels { get; set; }
    }
}
