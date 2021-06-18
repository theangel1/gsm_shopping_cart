using System;
using System.Collections.Generic;
using System.Text;
using GSM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GSM.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Building> Building { get; set; }
        public DbSet<Contract> Contract { get; set; }        
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
