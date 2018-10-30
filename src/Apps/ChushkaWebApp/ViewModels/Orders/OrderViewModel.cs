using System;

namespace ChushkaWebApp.ViewModels.Orders
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string ProductName { get; set; }

        public DateTime OrderedOn { get; set; }
    }
}