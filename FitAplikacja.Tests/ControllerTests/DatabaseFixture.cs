using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace FitAplikacja.Tests.ControllerTests
{
    // Klasa umozliwia wspoldzielenie bazy do testow
    public class DatabaseFixture : IDisposable
    {
        private static readonly string _connectionString = "Persist Security Info=True;Server=127.0.0.1,1433;Initial Catalog=FitAplikacjaTesting;User ID=sa;Password=eT98!5rtviVvZ^r*exfs;";
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public DbConnection Connection { get; }

        public DatabaseFixture()
        {
            Connection = new SqlConnection(_connectionString);

            Seed();
            Connection.Open();
        }

        public AppDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(Connection).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void Seed()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        // reset i wprowadzenie danych testowych
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        var product1 = new Product()
                        {
                            Name = "Product 1",
                            Calories = 1000
                        };

                        var product2 = new Product()
                        {
                            Name = "Another product",
                            Calories = 5736
                        };

                        var product3 = new Product()
                        {
                            Name = "Last product",
                            Calories = 9218
                        };

                        context.AddRange(product1, product2, product3);
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
