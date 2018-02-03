using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using System.Net.Http;
using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

namespace CarFinder.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Car> Car { get; set; }

        public async Task<List<string>> GetYears()
        {
            return await this.Database.SqlQuery<string>("GetAllYears").ToListAsync();
        }

        public async Task<List<string>> GetMakes(string year)
        {
            return await this.Database.SqlQuery<string>("MakesByYear @Year", new SqlParameter("year", year)).ToListAsync();
        }

        public async Task<List<string>> GetModels(string year, string make)
        {
            return await this.Database.SqlQuery<string>("ModelByYearAndMake @Year, @Make",
                new SqlParameter("year", year),
                new SqlParameter("make", make))
            .ToListAsync();
        }

        public async Task<List<string>> GetTrims(string year, string make, string model)
        {
            return await this.Database.SqlQuery<string>("TrimByYearMakeModel @Year, @Make, @Model",
                new SqlParameter("year", year),
                new SqlParameter("make", make),
                new SqlParameter("model", model))
            .ToListAsync();
        }

        public async Task<List<Car>> GetCarByYearMakeModelTrim(string year, string make, string model, string trim)
        {
            return await this.Database.SqlQuery<Car>("CarByYearMakeModelTrim @Year, @Make, @Model, @Trim",
                new SqlParameter("year", year),
                new SqlParameter("make", make),
                new SqlParameter("model", model),
                new SqlParameter("trim", trim))
              .ToListAsync();
        }

        public async Task<List<Car>> GetCarByYearMakeModel(string year, string make, string model)
        {
            return await this.Database.SqlQuery<Car>("CarByYearMakeModel @Year, @Make, @Model",
                new SqlParameter("year", year),
                new SqlParameter("make", make),
                new SqlParameter("model", model))
              .ToListAsync();
        }
    }
}