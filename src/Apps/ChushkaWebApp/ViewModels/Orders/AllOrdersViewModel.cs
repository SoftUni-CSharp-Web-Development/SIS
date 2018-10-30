using System.Collections.Generic;

namespace ChushkaWebApp.ViewModels.Orders
{
    public class AllOrdersViewModel
    {
        public IEnumerable<OrderViewModel> Orders { get; set; }
    }
}