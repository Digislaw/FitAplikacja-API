using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Workouts;
using FitAplikacja.Core.Dtos.Output.Exercises;
using FitAplikacja.Core.Dtos.Output.Workouts;
using FitAplikacja.Core.Models;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Controllers
{
    [Route("api/workouts")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class WorkoutsController : ControllerBase
    {
        private readonly IWorkoutRepository _workoutRepository;
        private readonly IExerciseRepository _exerciseRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public WorkoutsController(IWorkoutRepository workoutRepository, IExerciseRepository exerciseRepository, IAuthorizationService authorizationService, IMapper mapper)
        {
            _workoutRepository = workoutRepository ?? throw new NullReferenceException(nameof(workoutRepository));
            _exerciseRepository = exerciseRepository ?? throw new NullReferenceException(nameof(exerciseRepository));
            _authorizationService = authorizationService ?? throw new NullReferenceException(nameof(authorizationService));
            _mapper = mapper ?? throw new NullReferenceException(nameof(mapper));
        }

        #region GET

        /// <summary>
        /// Get workouts
        /// </summary>
        /// <param name="skip">Number of workouts to skip</param>
        /// <param name="take">Number of workouts to take</param>
        /// <returns>Array of workouts</returns>
        /// <response code="200">Returns the array of workouts</response>
        /// <response code="400">Incorrect parameters. Skip can't be a negative value. Take must be between 1 and 20.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkoutResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<WorkoutResponse>>> GetWorkouts([FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            if (skip < 0 || take <= 0 || take > 20)
            {
                return BadRequest("Incorrect parameters");
            }

            var workouts = await _workoutRepository.GetManyAsync(skip, take);
            return Ok(_mapper.Map<IEnumerable<WorkoutResponse>>(workouts));
        }

        /// <summary>
        /// Get the specified user's workouts
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="skip">Number of workouts to skip</param>
        /// <param name="take">Number of workouts to take</param>
        /// <returns>Array of workouts</returns>
        /// <response code="200">Returns the array of workouts</response>
        /// <response code="400">Incorrect parameters. Skip can't be a negative value. Take must be between 1 and 20.</response>
        /// <response code="403">User is not the owner and not an admin</response>
        [Authorize(Policy = "HasUserRouteAccess")]
        [HttpGet("~/api/users/{userId}/workouts")]
        [ProducesResponseType(typeof(IEnumerable<WorkoutResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<WorkoutResponse>>> GetUserWorkouts(string userId, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            if (skip < 0 || take <= 0 || take > 20)
            {
                return BadRequest("Incorrect parameters");
            }

            var workouts = await _workoutRepository.GetManyAsync(skip, take, userId);
            return Ok(_mapper.Map<IEnumerable<WorkoutResponse>>(workouts));
        }

        /// <summary>
        /// Get the specified workout
        /// </summary>
        /// <param name="id">Workout ID</param>
        /// <returns>The specified workout object</returns>
        /// <response code="200">Returns the specified workout object</response>
        /// <response code="403">The user is not the owner or an admin</response>
        /// <response code="404">Workout with the specified ID does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkoutResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkoutResponse>> GetWorkout(int id)
        {
            var workout = await _workoutRepository.GetOneAsync(id);

            if (workout == null)
            {
                return NotFound("The specified workout does not exist");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workout, "HasOwnerAccess");

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You don't have permissions to access this resource");
            }

            return Ok(_mapper.Map<WorkoutResponse>(workout));
        }

        /// <summary>
        /// Get the specified workout's exercises
        /// </summary>
        /// <param name="id">Workout ID</param>
        /// <returns>Array of exercises</returns>
        /// <response code="200">Returns the array of exercises</response>
        /// <response code="403">The user is not the owner or an admin</response>
        /// <response code="404">Workout with the specified ID does not exist</response>
        [HttpGet("{id}/exercises")]
        [ProducesResponseType(typeof(ExerciseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ExerciseResponse>>> GetWorkoutExercises(int id)
        {
            var workout = await _workoutRepository.GetOneAsync(id);

            if (workout == null)
            {
                return NotFound("The specified workout does not exist");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workout, "HasOwnerAccess");

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You don't have permissions to access this resource");
            }

            var exercisesDto = _mapper.Map<IEnumerable<ExerciseResponse>>(workout.Exercises);
            return Ok(exercisesDto);
        }

        #endregion

        #region POST

        /// <summary>
        /// Add a new workout for the current user
        /// </summary>
        /// <param name="dto">Workout data</param>
        /// <returns>The new workout</returns>
        /// <response code="201">Workout added successfully</response>
        /// <response code="500">Saving the workout to the database failed</response>
        [HttpPost]
        [ProducesResponseType(typeof(WorkoutResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<WorkoutResponse>> AddWorkout(WorkoutInputRequest dto)
        {
            var workout = _mapper.Map<Workout>(dto);

            workout.ApplicationUserId = this.GetCurrentUserId();
            var response = await _workoutRepository.SaveAsync(workout);

            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Saving the workout to the database failed");
            }

            var workoutToReturn = _mapper.Map<WorkoutResponse>(workout);
            return CreatedAtAction(nameof(GetWorkout), new { workoutToReturn.Id }, workoutToReturn);
        }

        /// <summary>
        /// Add the specified exercise to the workout
        /// </summary>
        /// <param name="workoutId">Workout ID</param>
        /// <param name="exerciseId">Exercise ID</param>
        /// <returns>Status code</returns>
        /// <response code="204">Exercise added successfully</response>
        /// <response code="403">The user is not the owner or an admin</response>
        /// <response code="404">Workout or exercise not found</response>
        /// <response code="500">Saving the workout to the database failed</response>
        [HttpPost("{workoutId}/exercises/{exerciseId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddExercise(int workoutId, int exerciseId)
        {
            var workout = await _workoutRepository.GetOneAsync(workoutId);

            if (workout == null)
            {
                return NotFound("Workout not found");
            }

            if (workout.ApplicationUserId != this.GetCurrentUserId() && !User.IsInRole("Admin"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You don't have permissions to access this resource");
            }

            var exercise = await _exerciseRepository.GetOneAsync(exerciseId);

            if (exercise == null)
            {
                return NotFound("Exercise not found");
            }

            if (workout.Exercises.FirstOrDefault(e => e.Id == exerciseId) == null)
            {
                workout.Exercises.Add(exercise);
                var response = await _workoutRepository.SaveAsync(workout);

                if (!response)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Saving the workout to the database failed");
                }
            }

            return NoContent();
        }


        #endregion

        #region PUT

        /// <summary>
        /// Update the specified workout
        /// </summary>
        /// <param name="id">Workout ID</param>
        /// <param name="dto">Workout properties</param>
        /// <returns>Status code</returns>
        /// <response code="204">The workout has been updated successfully</response>
        /// <response code="403">The user is not the owner or an admin</response>
        /// <response code="404">The specified workout does not exist</response>
        /// <response code="500">Saving changes to the database failed</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateWorkout(int id, WorkoutInputRequest dto)
        {
            var workout = await _workoutRepository.GetOneAsync(id);

            if (workout == null)
            {
                return NotFound("The specified workout does not exist");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workout, "HasOwnerAccess");

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You don't have permissions to access this resource");
            }

            _mapper.Map(dto, workout);
            var response = await _workoutRepository.SaveAsync(workout);

            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Saving changes to the database failed");
            }

            return NoContent();
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Delete the specified workout
        /// </summary>
        /// <param name="id">Workout ID</param>
        /// <returns>Status code</returns>
        /// <response code="204">The workout has been deleted successfully</response>
        /// <response code="403">The user is not the owner or an admin</response>
        /// <response code="404">The specified workout does not exist</response>
        /// <response code="500">Deleting the workout from the databse failed</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteWorkout(int id)
        {
            var workout = await _workoutRepository.GetOneAsync(id);

            if (workout == null)
            {
                return NotFound("The specified workout does not exist");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workout, "HasOwnerAccess");

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You don't have permissions to access this resource");
            }

            var response = await _workoutRepository.DeleteAsync(workout);

            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Deleting the workout from the databse failed");
            }

            return NoContent();
        }

        #endregion
    }
}
