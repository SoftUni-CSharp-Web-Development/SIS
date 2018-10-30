using System;
using System.Collections.Generic;
using System.Text;
using ChushkaWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChushkaWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=Chushka;Integrated Security=True;");
        }
    }
}
