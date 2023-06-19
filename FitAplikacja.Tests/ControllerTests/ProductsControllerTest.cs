using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Products;
using FitAplikacja.Core.Dtos.Output.Products;
using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure.Repositories.Concrete;
using FitAplikacjaAPI.Controllers;
using FitAplikacjaAPI.Profiles;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FitAplikacja.Tests.ControllerTests
{
    public class ProductsControllerTest : IClassFixture<DatabaseFixture>
    {
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public ProductsControllerTest(DatabaseFixture fixture)
        {
            _fixture = fixture;

            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new ProductsProfile());
            });

            _mapper = mapperConfig.CreateMapper();
        }


        #region GET

        [Fact]
        public async Task Can_get_product()
        {
            using (var context = _fixture.CreateContext())
            {

                var controller = new ProductsController(new ProductRepository(context), _mapper);
                var result = (await controller.GetProduct(2)).Result as ObjectResult;
                var product = result.Value as ProductResponse;

                Assert.Equal("Another product", product.Name);
            }
        }

        [Fact]
        public async Task Can_get_many_products()
        {
            using (var context = _fixture.CreateContext())
            {
                var controller = new ProductsController(new ProductRepository(context), _mapper);
                var result = (await controller.GetProducts()).Result as ObjectResult;
                var products = result.Value as ICollection<ProductResponse>;

                Assert.Equal(3, products.Count);
            }
        }

        #endregion

        #region POST

        [Fact]
        public async void Can_add_product()
        {
            string name = "New product";

            using (var transaction = _fixture.Connection.BeginTransaction())
            {
                using (var context = _fixture.CreateContext(transaction))
                {
                    var controller = new ProductsController(new ProductRepository(context), _mapper);

                    var product = new ProductRequest()
                    {
                        Name = name,
                        Calories = 9999
                    };

                    var result = (await controller.AddProduct(product)).Result as ObjectResult;
                    var newProduct = result.Value as ProductResponse;

                    Assert.Equal(name, newProduct.Name);
                }

                using (var context = _fixture.CreateContext(transaction))
                {
                    var product = context.Set<Product>().Single(p => p.Name == "New product");
                    Assert.Equal(name, product.Name);
                }
            }
        }

        #endregion

        #region PUT

        [Fact]
        public async void Can_update_product()
        {
            int id = 2;
            string name = "Updated product";

            using (var transaction = _fixture.Connection.BeginTransaction())
            {
                using (var context = _fixture.CreateContext(transaction))
                {
                    var controller = new ProductsController(new ProductRepository(context), _mapper);

                    var product = new ProductRequest()
                    {
                        Name = name,
                        Calories = 1234
                    };

                    await controller.UpdateProduct(id, product);

                    var result = (await controller.GetProduct(id)).Result as ObjectResult;
                    var updatedProduct = result.Value as ProductResponse;

                    Assert.Equal(name, updatedProduct.Name);
                }

                using (var context = _fixture.CreateContext(transaction))
                {
                    var product = context.Set<Product>().Single(p => p.Name == name);
                    Assert.Equal(name, product.Name);
                }
            }
        }

        #endregion

        #region DELETE

        [Fact]
        public async void Can_delete_product()
        {
            int id = 1;

            using (var transaction = _fixture.Connection.BeginTransaction())
            {
                using (var context = _fixture.CreateContext(transaction))
                {
                    var controller = new ProductsController(new ProductRepository(context), _mapper);
                    await controller.DeleteProduct(id);
                }

                using (var context = _fixture.CreateContext(transaction))
                {
                    Assert.False(context.Set<Product>().Any(p => p.Id == id));
                }
            }
        }

        #endregion
    }
}
