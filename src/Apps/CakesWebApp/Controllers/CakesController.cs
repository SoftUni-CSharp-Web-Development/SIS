using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CakesWebApp.Extensions;
using CakesWebApp.Models;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public class CakesController : BaseController
    {
        public IHttpResponse AddCakes(IHttpRequest request)
        {
            return this.View("AddCakes");
        }

        public IHttpResponse DoAddCakes(IHttpRequest request)
        {
            var name = request.FormData["name"].ToString().Trim().UrlDecode();
            var price = decimal.Parse(request.FormData["price"].ToString().UrlDecode());
            var picture = request.FormData["picture"].ToString().Trim().UrlDecode();

            // TODO: Validation

            var product = new Product
            {
                Name = name,
                Price = price,
                ImageUrl = picture
            };
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
            return new RedirectResult("/");
        }

        public IHttpResponse ById(IHttpRequest request)
        {
            var id = int.Parse(request.QueryData["id"].ToString());
            var product = this.Db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return this.BadRequestError("Cake not found.");
            }

            var viewBag = new Dictionary<string, string>
            {
                {"Name", product.Name},
                {"Price", product.Price.ToString(CultureInfo.InvariantCulture)},
                {"ImageUrl", product.ImageUrl}
            };
            return this.View("CakeById", viewBag);
        }
    }
}
