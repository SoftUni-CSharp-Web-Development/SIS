using System;
using System.Linq;
using CakesWebApp.Models;
using CakesWebApp.ViewModels.Cakes;
using SIS.HTTP.Responses;
using SIS.MvcFramework;
using SIS.MvcFramework.Logger;

namespace CakesWebApp.Controllers
{
    public class CakesController : BaseController
    {
        private readonly ILogger logger;

        public CakesController(ILogger logger)
        {
            this.logger = logger;
        }

        [HttpGet("/cakes/add")]
        public IHttpResponse AddCakes()
        {
            return this.View("AddCakes");
        }

        [HttpPost("/cakes/add")]
        public IHttpResponse DoAddCakes(DoAddCakesInputModel model)
        {
            // TODO: Validation
            var product = model.To<Product>();
            this.Db.Products.Add(product);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO: Log error
                return this.ServerError(e.Message);
            }

            // Redirect
            return this.Redirect("/cakes/view?id=" + product.Id);
        }

        // cakes/view?id=1
        [HttpGet("/cakes/view")]
        public IHttpResponse ById(int id)
        {
            var product = this.Db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return this.BadRequestError("Cake not found.");
            }

            var viewModel = product.To<CakeViewModel>();
            return this.View("CakeById", viewModel);
        }

        // cakes/search?searchText=cake
        [HttpGet("/cakes/search")]
        public IHttpResponse Search(string searchText)
        {
            var cakes = this.Db.Products.Where(x => x.Name.Contains(searchText))
                .Select(x => new CakeViewModel
                             {
                                 Id = x.Id,
                                 Name = x.Name,
                                 ImageUrl = x.ImageUrl,
                                 Price = x.Price,
                             }).ToList();
            var cakesViewModel = new SearchViewModel
            {
                Cakes = cakes,
                SearchText = searchText,
            };

            return this.View("Search", cakesViewModel);
        }
    }
}
