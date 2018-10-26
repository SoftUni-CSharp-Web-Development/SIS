using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.MvcFramework.ViewEngine
{
    public class UserModel
    {
        public UserModel()
        {
            this.Name = string.Empty;
            this.Role = string.Empty;
            this.Exist = false;
        }

        public string Name { get; set; }

        public string Role { get; set; }

        public bool Exist { get; set; }
    }
}