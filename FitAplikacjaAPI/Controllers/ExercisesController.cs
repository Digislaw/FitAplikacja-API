using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Exercises;
using FitAplikacja.Core.Dtos.Output.Exercises;
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
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/exercises")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseRepository _repository;
        private readonly IMapper _mapper;

        public ExercisesController(IExerciseRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region GET

        /// <summary>
        /// Get the specified exercise
        /// </summary>
        /// <param name="id">Exercise ID</param>
        /// <returns>The specified exercise object</returns>
        /// <response code="200">Returns the specified exercise object</response>
        /// <response code="404">Exercise with the specified ID does not exist</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ExerciseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExerciseResponse>> GetExercise(int id)
        {
            var exercise = await _repository.GetOneAsync(id);

            if(exercise == null)
            {
                return NotFound("Exercise with the specified ID does not exist");
            }

            return Ok(_mapper.Map<ExerciseResponse>(exercise));
        }

        /// <summary>
        /// Get exercises
        /// </summary>
        /// <param name="skip">Number of exercises to skip</param>
        /// <param name="take">Number of exercises to take</param>
        /// <returns>Array of exercises</returns>
        /// <response code="200">Returns the array of exercises</response>
        /// <response code="400">Incorrect parameters. Skip can't be a negative value. Take must be between 1 and 20.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ExerciseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ExerciseResponse>>> GetExercises([FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            if (skip < 0 || take <= 0 || take > 20)
            {
                return BadRequest("Incorrect parameters");
            }

            var exercises = await _repository.GetManyAsync(skip, take);
            return Ok(_mapper.Map<IEnumerable<ExerciseResponse>>(exercises));
        }

        #endregion

        #region POST 

        /// <summary>
        /// Add a new exercise
        /// </summary>
        /// <param name="dto">Exercise data</param>
        /// <returns>The new exercise</returns>
        /// <response code="201">Exercise added successfully</response>
        /// <response code="500">Saving the exercise to the database failed</response>
        [HttpPost]
        [ProducesResponseType(typeof(ExerciseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExerciseResponse>> AddExercise(ExerciseRequest dto)
        {
            var exercise = _mapper.Map<Exercise>(dto);

            var response = await _repository.SaveAsync(exercise);

            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Saving the exercise to the database failed");
            }

            var exerciseToReturn = _mapper.Map<ExerciseResponse>(exercise);
            return CreatedAtAction(nameof(GetExercise), new { exerciseToReturn.Id }, exerciseToReturn);
        }


        #endregion

        #region PUT

        /// <summary>
        /// Update the specified exercise
        /// </summary>
        /// <param name="id">Exercise ID</param>
        /// <param name="dto">Exercise properties</param>
        /// <returns>Status code</returns>
        /// <response code="204">The exercise has been updated successfully</response>
        /// <response code="404">The specified exercise does not exist</response>
        /// <response code="500">Saving changes to the database failed</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateExercise(int id, ExerciseRequest dto)
        {
            var exercise = await _repository.GetOneAsync(id);

            if (exercise == null)
            {
                return NotFound("The specified exercise does not exist");
            }

            _mapper.Map(dto, exercise);
            var response = await _repository.SaveAsync(exercise);

            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Saving changes to the database failed");
            }

            return NoContent();
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Delete the specified exercise
        /// </summary>
        /// <param name="id">Exercise ID</param>
        /// <returns>Status code</returns>
        /// <response code="204">The exercise has been deleted successfully</response>
        /// <response code="404">The specified exercise does not exist</response>
        /// <response code="500">Deleting the exercise from the databse failed</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteExercise(int id)
        {
            var exercise = await _repository.GetOneAsync(id);

            if (exercise == null)
            {
                return NotFound("The specified exercise does not exist");
            }

            var response = await _repository.DeleteAsync(exercise);

            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Deleting the exercise from the databse failed");
            }

            return NoContent();
        }

        #endregion
    }
}
