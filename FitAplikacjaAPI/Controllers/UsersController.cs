using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Users;
using FitAplikacja.Core.Dtos.Output.Users;
using FitAplikacjaAPI.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        #region GET

        /// <summary>
        /// Get many users
        /// </summary>
        /// <param name="skip">Skip specified number of users</param>
        /// <param name="take">Take specified number of users</param>
        /// <returns>Array of users</returns>
        /// <response code="200">OK</response>
        /// <response code="400">Incorrect parameters. Skip can't be a negative value. Take must be between 1 and 20.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<ApplicationUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ApplicationUserResponse>>> GetUsers([FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            if(skip < 0 || take <= 0 || take > 20)
            {
                return BadRequest("Incorrect parameters");
            }

            var users = await _userService.GetMany(skip, take);
            return Ok(_mapper.Map<IEnumerable<ApplicationUserResponse>>(users));
        }

        /// <summary>
        /// Get the specified user
        /// </summary>
        /// <param name="id">User ID string</param>
        /// <returns>The specified user's data</returns>
        /// <response code="200">OK</response>
        /// <response code="404">User with the specified ID string does not exist</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApplicationUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationUserResponse>> GetUser(string id)
        {
            var user = await _userService.GetByIdAsync(id);

            if(user == null)
            {
                return NotFound("User with the specified ID string does not exist");
            }

            return Ok(_mapper.Map<ApplicationUserResponse>(user));
        }

        /// <summary>
        /// Get the specified user's app data (User Access)
        /// </summary>
        /// <param name="userId">User ID string</param>
        /// <returns>The specified user's app data (e.g. age, weight)</returns>
        [HttpGet("{userId}/details")]
        [Authorize(Policy = "HasUserRouteAccess")]
        [ProducesResponseType(typeof(ApplicationUserDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationUserDetailsResponse>> GetUserDetails(string userId)
        {
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User with the specified ID string does not exist");
            }

            return Ok(_mapper.Map<ApplicationUserDetailsResponse>(user));
        }

        /// <summary>
        /// Search for users by e-mail and/or name
        /// </summary>
        /// <param name="email">User e-mail address</param>
        /// <param name="username">Username</param>
        /// <returns>Array of found users</returns>
        /// <response code="200">Returns array of found users</response>
        /// <response code="400">Queries are empty or null</response>
        /// <response code="404">No user was found</response>
        /// <response code="500">Error while searching the database</response>
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<ApplicationUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ApplicationUserResponse>>> SearchUser(string email, string username)
        {
            if(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(username))
            {
                return BadRequest("At least one of the parameters is required");
            }

            try
            {
                var result = await _userService.SearchAsync(email, username);

                if (result.Any())
                {
                    return Ok(_mapper.Map<IEnumerable<ApplicationUserResponse>>(result));
                }
                else
                {
                    return NotFound("No users found");
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while searching the database");
            }
        }

        /// <summary>
        /// Get roles of the specified user
        /// </summary>
        /// <param name="id">User ID string</param>
        /// <returns>Array of user's roles</returns>
        /// <response code="200">Array of user's roles</response>
        /// <response code="404">The specified user does not exist</response>
        [HttpGet("{id}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string id)
        {
            var user = await _userService.GetByIdAsync(id);

            if(user == null)
            {
                return NotFound("The specified user does not exist");
            }

            var roles = await _userService.GetUserRolesAsync(user);
            return Ok(roles);
        }

        #endregion

        #region POST

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">User registration request</param>
        /// <returns>Status code</returns>
        /// <response code="200">Registration was successful</response>
        /// <response code="400">User with this e-mail address already exists</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticationSucessResponse>> Register(UserRegistrationRequest request)
        {
            var authResult = await _userService
                .RegisterAsync(request.Email, request.Username, request.Password);

            if (!authResult.Success)
            {
                return BadRequest(new AuthenticationFailedResponse
                {
                    Errors = authResult.Errors
                });
            }

            AppendRefreshTokenCookie(authResult.RefreshToken.Token);

            return Ok(new AuthenticationSucessResponse
            {
                Token = authResult.Token,
                RefreshTokenExpiration = authResult.RefreshToken.Expires
            });
        }

        /// <summary>
        /// Authenticate the specified user
        /// </summary>
        /// <param name="request">User login request</param>
        /// <returns>JSON Web Token and refresh token's expiration date</returns>
        /// <response code="200">Authentication was successful</response>
        /// <response code="400">Incorrect e-mail or password</response>
        [HttpPost("token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticationSucessResponse>> Login(UserLoginRequest request)
        {
            var authResult = await _userService.LoginAsync(request.Email, request.Password);

            if (!authResult.Success)
            {
                return BadRequest(new AuthenticationFailedResponse
                {
                     Errors = authResult.Errors
                });
            }

            AppendRefreshTokenCookie(authResult.RefreshToken.Token);

            return Ok(new AuthenticationSucessResponse
            {
                UserId = authResult.UserId,
                Token = authResult.Token,
                RefreshTokenExpiration = authResult.RefreshToken.Expires
            });
        }

        /// <summary>
        /// Authenticate the specified Facebook user
        /// </summary>
        /// <param name="request">Facebook user login request</param>
        /// <returns>JSON Web Token</returns>
        /// <response code="200">Authentication was successful</response>
        /// <response code="400">Access token is not valid</response>
        [HttpPost("token/facebook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticationSucessResponse>> LoginFacebook(UserFacebookLoginRequest request)
        {
            var authResult = await _userService.AuthenticateFacebookAsync(request.AccessToken);

            if (!authResult.Success)
            {
                return BadRequest(new AuthenticationFailedResponse
                {
                    Errors = authResult.Errors
                });
            }

            return Ok(new AuthenticationSucessResponse
            {
                UserId = authResult.UserId,
                Token = authResult.Token,
                RefreshTokenExpiration = authResult.RefreshToken.Expires
            });
        }

        /// <summary>
        /// Authenticate Google user
        /// </summary>
        /// <param name="request">Google user login request</param>
        /// <returns>JSON Web Token</returns>
        /// <response code="200">Authentication was successful</response>
        /// <response code="400">Access token is not valid</response>
        [HttpPost("token/google")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticationSucessResponse>> LoginGoogle(UserGoogleLoginRequest request)
        {
            var authResult = await _userService.AuthenticateGoogleAsync(request.IdToken);

            if (!authResult.Success)
            {
                return BadRequest(new AuthenticationFailedResponse
                {
                    Errors = authResult.Errors
                });
            }

            return Ok(new AuthenticationSucessResponse
            {
                UserId = authResult.UserId,
                Token = authResult.Token,
                RefreshTokenExpiration = authResult.RefreshToken.Expires
            });
        }

        /// <summary>
        /// Refresh JSON Web Token
        /// </summary>
        /// <returns>JSON Web Token and refresh token's expiration date</returns>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="400">Incorrect refresh token data or cookie does not exist</response>
        [HttpPost("token/refresh")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticationSucessResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if(refreshToken == null)
            {
                return BadRequest("Refresh token can't be null");
            }

            var result = await _userService.RefreshTokenAsync(refreshToken);

            if(!result.Success)
            {
                return BadRequest(new AuthenticationFailedResponse
                {
                    Errors = result.Errors
                });
            }

            Response.Cookies.Delete("refreshToken");
            AppendRefreshTokenCookie(result.RefreshToken.Token);

            return Ok(new AuthenticationSucessResponse
            {
                Token = result.Token,
                RefreshTokenExpiration = result.RefreshToken.Expires
            });
        }

        #endregion

        #region PUT

        /// <summary>
        /// Set the specified user's roles. Empty array will clear user's roles
        /// </summary>
        /// <param name="id">User ID string</param>
        /// <param name="roles">Array of the named roles</param>
        /// <returns>Status code</returns>
        /// <response code="204">User's roles changed successfully</response>
        /// <response code="404">The specified user does not exist</response>
        /// <response code="500">User roles update failed. Roles do not exist?</response>
        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserRoles(string id, IEnumerable<string> roles)
        {
            var user = await _userService.GetByIdAsync(id);

            if(user == null)
            {
                return NotFound("The specified user does not exist");
            }

            var response = await _userService.SetUserRolesAsync(user, roles);

            if(!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "User roles update failed. Roles do not exist?");
            }

            return NoContent();
        }

        /// <summary>
        /// Update the specified user's app data (User Access)
        /// </summary>
        /// <param name="userId">User ID string</param>
        /// <param name="dto">Updated user app data</param>
        /// <returns>Status code</returns>
        /// <response code="204">User's updated successfully</response>
        /// <response code="404">The specified user does not exist</response>
        /// <response code="500">User update failed while saving changes to the database</response>
        [HttpPut("{userId}/details")]
        [Authorize(Policy = "HasUserRouteAccess")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserDetails(string userId, ApplicationUserDetailsRequest dto)
        {
            var user = await _userService.GetByIdAsync(userId);

            if(user == null)
            {
                return NotFound("The specified user does not exist");
            }

            _mapper.Map(dto, user);
            var result = await _userService.UpdateUser(user);

            if(!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Saving changes failed");
            }

            return NoContent();
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Delete specified user
        /// </summary>
        /// <param name="id">User ID string</param>
        /// <returns>Status code</returns>
        /// <response code="204">User has been deleted successfully</response>
        /// <response code="404">The specified user does not exist</response>
        /// <response code="500">Failed to delete the user from database</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userService.GetByIdAsync(id);

            if(user == null)
            {
                return NotFound("The specified user does not exist");
            }

            var success = await _userService.DeleteAsync(user);

            if(!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete the user from database");
            }

            return NoContent();
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Adds refresh token cookie
        /// </summary>
        /// <param name="refreshToken">Refresh token string</param>
        private void AppendRefreshTokenCookie(string refreshToken)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true
            };

            Response.Cookies.Append("refreshToken", refreshToken, options);
        }

        #endregion
    }
}
