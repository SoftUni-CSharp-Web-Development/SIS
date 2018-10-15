using System.Collections.Generic;

namespace IRunesWebApp
{
    public class IndexViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}
