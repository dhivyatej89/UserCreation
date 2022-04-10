using Microsoft.EntityFrameworkCore;
using Sample.Models.Entities;

namespace Sample.Repository
{
    public class SampleDBContext : DbContext
    {
        public SampleDBContext() { }

        public SampleDBContext(DbContextOptions<SampleDBContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {          
                optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-OS08OBG\SQLEXPRESS;Initial Catalog=MasterLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");           
        }

        public DbSet<User> Users { get; set; }
    }
}
