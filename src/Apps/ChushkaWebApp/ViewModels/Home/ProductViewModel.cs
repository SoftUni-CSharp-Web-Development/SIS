using System;
using System.Text;

namespace ChushkaWebApp.ViewModels.Home
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortDescription
        {
            get
            {
                if (this.Description?.Length > 50)
                {
                    return this.Description.Substring(0, 50) + "...";
                }
                else
                {
                    return this.Description;
                }
            }
        }

        public decimal Price { get; set; }
    }
}
