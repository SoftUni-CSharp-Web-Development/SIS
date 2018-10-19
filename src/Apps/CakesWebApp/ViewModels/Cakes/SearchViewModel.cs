using System.Collections.Generic;

namespace CakesWebApp.ViewModels.Cakes
{
    public class SearchViewModel
    {
        public List<CakeViewModel> Cakes { get; set; }

        public string SearchText { get; set; }
    }
}