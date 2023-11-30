using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessorService.DB
{
    public class XMLDataContext : DbContext
    {
        public DbSet<RapidControlStatus> status { get; set; }

        public XMLDataContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=XMLDocDb.db"); // Путь к файлу базы
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
