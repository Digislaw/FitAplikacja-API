using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Products;
using FitAplikacja.Core.Dtos.Output.AssignedProducts;
using FitAplikacja.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AssignedProductsController : ControllerBase
    {
        private readonly IAssignedProductService _assignedProductService;
        private readonly IMapper _mapper;

        public AssignedProductsController(IAssignedProductService assignedProductService, IMapper mapper)
        {
            _assignedProductService = assignedProductService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get product assignments of the user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="skip">Number of assignments to skip</param>
        /// <param name="take">Number of assignments to take</param>
        /// <param name="date">Filter by date e.g. 2021-01-01. If not specified, the controller shows all assigned products without duplicates.</param>
        /// <returns>Array of products</returns>
        /// <response code="200">Returns the array of assignments</response>
        /// <response code="400">Incorrect parameters. Skip can't be a negative value. Take must be between 1 and 20.</response>
        /// <response code="403">Selected user ID does not match the logged user ID</response>
        [Authorize(Policy = "HasUserRouteAccess")]
        [HttpGet("~/api/users/{userId}/products")]
        [ProducesResponseType(typeof(IEnumerable<AssignedProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<AssignedProductResponse>>> GetUserProducts(string userId, [FromQuery] int skip = 0, [FromQuery] int take = 50, [FromQuery] DateTime? date = null)
        {
            if (skip < 0 || take <= 0 || take > 50)
            {
                return BadRequest("Incorrect parameters");
            }

            var assignedProducts = await _assignedProductService.GetAssignedProductsAsync(skip, take, userId, date);
            return Ok(_mapper.Map<IEnumerable<AssignedProductResponse>>(assignedProducts));
        }

        /// <summary>
        /// Assign a product to the specified user on the selected date
        /// </summary>
        /// <returns>Status code</returns>
        /// <response code="204">Product assigned successfully</response>
        /// <response code="403">Selected user ID does not match the logged user ID</response>
        /// <response code="404">The specified product does not exist</response>
        /// <response code="500">Error while saving changes to the database</response>
        [HttpPut("~/api/users/{userId}/product")]
        [Authorize(Policy = "HasUserRouteAccess")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignProduct(string userId, AssignedProductRequest dto)
        {
            var product = await _assignedProductService.GetProductAsync(dto.ProductId);

            if (product == null)
            {
                return NotFound("The specified product does not exist");
            }

            var response = await _assignedProductService
                .AssignProductAsync(userId, product, dto.Date, dto.Weight, dto.Count);

            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        /// <summary>
        /// Unassign the specified product from the selected user and date
        /// </summary>
        /// <returns>Status code</returns>
        /// <response code="204">Product unassigned successfully</response>
        /// <response code="403">Selected user ID does not match the logged user ID</response>
        /// <response code="404">The specified product or assignment does not exist</response>
        /// <response code="500">Error while saving changes to the database</response>
        [HttpDelete("~/api/users/{userId}/product/{assignedProductId}")]
        [Authorize(Policy = "HasUserRouteAccess")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnassignProduct(string userId, int assignedProductId, [FromQuery] DateTime date)
        {
            var assignedProduct = await _assignedProductService.GetAssignedProductAsync(userId, date, assignedProductId);

            if (assignedProduct == null)
            {
                return NotFound("The specified assignment does not exist");
            }

            var response = await _assignedProductService.UnassignProductAsync(assignedProduct);
            
            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }
    }
}
