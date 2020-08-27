using FilmReview.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace FilmReview.DAL
{
    public class mainContext : DbContext
    {
        public mainContext() : base("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=FilmRatedb; Integrated Security=SSPI;MultipleActiveResultSets=True")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}