﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CakesWebApp.Models;
using CakesWebApp.ViewModels.Cakes;
using SIS.HTTP.Requests;
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
            return this.View("AddCakes", "CakesViews");
        }

        [HttpPost("/cakes/add")]
        public IHttpResponse DoAddCakes(DoAddCakesInputModel model)
        {
            // TODO: Validation
            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                ImageUrl = model.Picture,
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
            return this.Redirect("/");
        }

        // cakes/view?id=1
        [HttpGet("/cakes/view")]
        public IHttpResponse ById()
        {
            var id = int.Parse(this.Request.QueryData["id"].ToString());
            var product = this.Db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return this.BadRequestError("Cake not found.");
            }

            var viewModel = new ByIdViewModel
            {
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
            };
            return this.View("CakeById", viewModel, "CakesViews");
        }

        public class ByIdViewModel
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
