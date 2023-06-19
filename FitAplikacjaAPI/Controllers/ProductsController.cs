using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Products;
using FitAplikacja.Core.Dtos.Output.Products;
using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IAssignedProductRepository _assignedProductRepo;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository repository, IAssignedProductRepository assignedProductRepository, IAuthorizationService authorizationService, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _assignedProductRepo = assignedProductRepository ?? throw new ArgumentNullException(nameof(assignedProductRepository));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region GET

        /// <summary>
        /// Get products
        /// </summary>
        /// <param name="skip">Number of products to skip</param>
        /// <param name="take">Number of products to take</param>
        /// <returns>Array of products</returns>
        /// <response code="200">Returns the array of products</response>
        /// <response code="400">Incorrect parameters. Skip can't be a negative value. Take must be between 1 and 20.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (skip < 0 || take <= 0 || take > 50)
            {
                return BadRequest("Incorrect parameters");
            }

            var products = await _repository.GetManyAsync(skip, take);
            return Ok(_mapper.Map<IEnumerable<ProductResponse>>(products));
        }

        /// <summary>
        /// Get the specified product info
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>The specified product object</returns>
        /// <response code="200">Returns the specified product object</response>
        /// <response code="404">Product with the specified ID does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            var product = await _repository.GetOneAsync(id);

            if (product == null)
            {
                return NotFound("Product with the specified ID does not exist");
            }

            return Ok(_mapper.Map<ProductResponse>(product));
        }

        /// <summary>
        /// Search products by name and/or barcode
        /// </summary>
        /// <param name="name">The name of products (LIKE query)</param>
        /// <param name="barcode">The barcode of products (exact query)</param>
        /// <returns>List of products</returns>
        /// <response code="200">Search query executed sucessfully</response>
        /// <response code="400">Incorrect search query</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> Search(
            [FromQuery] string name = null,
            [FromQuery] string barcode = null)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(barcode))
            {
                return BadRequest("Incorrect query");
            }

            var products = await _repository.SearchAsync(name, barcode);

            return Ok(_mapper.Map<IEnumerable<ProductResponse>>(products));
        }

        #endregion

        #region POST

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="product">Product data</param>
        /// <returns>The new product</returns>
        /// <response code="201">Product added successfully</response>
        /// <response code="500">Saving the product to the database failed</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductResponse>> AddProduct(ProductRequest product)
        {
            var productToAdd = _mapper.Map<Product>(product);
            var response = await _repository.SaveAsync(productToAdd);

            if(!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Saving the product to the database failed");
            }

            var productToReturn = _mapper.Map<ProductResponse>(productToAdd);
            return CreatedAtAction(nameof(GetProduct), new { productToReturn.Id }, productToReturn);
        }

        #endregion

        #region PUT

        /// <summary>
        /// Update the specified product (Admin Access)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="product">Product properties</param>
        /// <returns>Status code</returns>
        /// <response code="204">The product has been updated successfully</response>
        /// <response code="403">The user is not an admin</response>
        /// <response code="404">The specified product does not exist</response>
        /// <response code="500">Saving changes to the database failed</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateProduct(int id, ProductRequest product)
        {
            var productFromRepo = await _repository.GetOneAsync(id);

            if (productFromRepo == null)
            {
                return NotFound("The specified product does not exist");
            }

            // update the product & save changes
            _mapper.Map(product, productFromRepo);
            var response = await _repository.SaveAsync(productFromRepo);

            if(!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Saving changes to the database failed");
            }

            return NoContent();
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Delete the specified product (Admin Access)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Status code</returns>
        /// <response code="204">The product has been deleted successfully</response>
        /// <response code="403">The user is not an admin</response>
        /// <response code="404">The specified product does not exist</response>
        /// <response code="500">Deleting the product from the databse failed</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var productFromRepo = await _repository.GetOneAsync(id);

            if (productFromRepo == null)
            {
                return NotFound("The specified product does not exist");
            }

            // user is an admin -> can safely delete the product
            var response = await _repository.DeleteAsync(productFromRepo);

            if(!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Deleting the product from the databse failed");
            }

            return NoContent();
        }

        #endregion
    }
}
