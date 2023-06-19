using FitAplikacja.Core.Models;
using FitAplikacja.Core.Models.Services;
using FitAplikacja.Infrastructure;
using FitAplikacja.Services.Abstract;
using FitAplikacjaAPI.Services.Abstract;
using FitAplikacjaAPI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly JWTSettings _jwt;

        public UserService(AppDbContext context, 
            UserManager<ApplicationUser> userManager, 
            IFacebookAuthService facebookAuthService,
            IGoogleAuthService googleAuthService,
            IOptions<JWTSettings> jwt)
        {
            _context = context;
            _userManager = userManager;
            _facebookAuthService = facebookAuthService;
            _googleAuthService = googleAuthService;
            _jwt = jwt.Value;
        }

        #region Users

        /// <summary>
        /// Get user by ID string
        /// </summary>
        /// <param name="id">User ID string</param>
        /// <returns>The task object containing user entity</returns>
        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        /// <summary>
        /// Get many user entities
        /// </summary>
        /// <param name="skip">Number of users to skip</param>
        /// <param name="take">Number of users to take</param>
        /// <returns>The task object containing user entities</returns>
        public async Task<IEnumerable<ApplicationUser>> GetMany(int skip = 0, int take = 20)
        {
            return await _context.Users.OrderBy(u => u.Id).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> SearchAsync(string email, string username)
        {
            IQueryable<ApplicationUser> query = _context.Users;

            if(!string.IsNullOrEmpty(username))
            {
                query = query.Where(u => u.UserName == username);
            }

            if(!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email == email);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Create new user in the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>The task object containing successful registration flag</returns>
        public async Task<AuthenticationResult> RegisterAsync(string email, string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with the specified e-mail address already exists" }
                };
            }

            var newUser = new ApplicationUser()
            {
                Email = email,
                UserName = username
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            var token = await CreateTokenAsync(newUser);

            return new AuthenticationResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = await GetRefreshTokenAsync(newUser)
            };
        }

        /// <summary>
        /// Update the specified user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>The task object containing success flag</returns>
        public async Task<bool> UpdateUser(ApplicationUser user)
        {
            if (user == null)
            {
                return false;
            }

            IdentityResult result;

            try
            {
                result = await _userManager.UpdateAsync(user);
            }
            catch(Exception)
            {
                return false;
            }

            return result.Succeeded;
        }

        /// <summary>
        /// Delete specified user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>The task object containing success flag</returns>
        public async Task<bool> DeleteAsync(ApplicationUser user)
        {
            if(user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        #endregion

        #region Tokens

        /// <summary>
        /// Authenticate user with password
        /// </summary>
        /// <param name="user">User Identity</param>
        /// <param name="password">User password</param>
        /// <returns>The task object containing serialized JWT as string</returns>
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Incorrect e-mail or password" }
                };
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if(!isValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Incorrect e-mail or password" }
                };
            }

            var token = await CreateTokenAsync(user);

            return new AuthenticationResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = await GetRefreshTokenAsync(user),
                UserId = user.Id
            };
        }

        /// <summary>
        /// Authenticate Facebook user
        /// </summary>
        /// <param name="accessToken">Facebook access token</param>
        /// <returns>Authentication result</returns>
        public async Task<AuthenticationResult> AuthenticateFacebookAsync(string accessToken)
        {
            var validationResult = await _facebookAuthService.ValidateTokenAsync(accessToken);

            if (!validationResult.Data.IsValid)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "The specified access token is not valid" }
                };
            }

            var userData = await _facebookAuthService.GetUserDataAsync(accessToken);
            var user = await _userManager.FindByEmailAsync(userData.Email);

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    Email = userData.Email,
                    UserName = userData.FirstName
                };

                var registrationResult = await _userManager.CreateAsync(user);

                if (!registrationResult.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = registrationResult.Errors.Select(e => e.Description)
                    };
                }
            }

            var token = await CreateTokenAsync(user);

            return new AuthenticationResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = await GetRefreshTokenAsync(user),
                UserId = user.Id
            };
        }

        /// <summary>
        /// Authenticate Google user
        /// </summary>
        /// <param name="idToken">Google ID Token</param>
        /// <returns>Authentication result</returns>
        public async Task<AuthenticationResult> AuthenticateGoogleAsync(string idToken)
        {
            var validationResult = await _googleAuthService.ValidateTokenAsync(idToken);

            if (!validationResult.IsValid)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "The specified ID token is not valid" }
                };
            }

            var user = await _userManager.FindByEmailAsync(validationResult.Email);

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    Email = validationResult.Email,
                    UserName = validationResult.Name
                };

                var registrationResult = await _userManager.CreateAsync(user);

                if (!registrationResult.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = registrationResult.Errors.Select(e => e.Description)
                    };
                }
            }

            var token = await CreateTokenAsync(user);

            return new AuthenticationResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = await GetRefreshTokenAsync(user),
                UserId = user.Id
            };
        }

        /// <summary>
        /// Create new JSON Web Token
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>The task object containing JWT object</returns>
        private async Task<JwtSecurityToken> CreateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roleClaims = new List<Claim>();

            for(int i = 0; i < roles.Count; i++)
            {
                // add roles to "roles" claim
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            // merge all claims
            var claims = new[]
            {                
                // subject claim
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),

                // JWT ID claim
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
                .Union(roleClaims)
                .Union(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // create token
            var token = new JwtSecurityToken(
                    issuer: _jwt.Issuer,
                    audience: _jwt.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwt.Duration),
                    signingCredentials: signingCredentials
            );

            return token;
        }

        /// <summary>
        /// Get valid refresh token
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>The task object containing a refresh token</returns>
        private async Task<RefreshToken> GetRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = user.RefreshToken;

            if(refreshToken == null || refreshToken.IsExpired)
            {
                var newRefreshToken = CreateRefreshToken(user);
                user.RefreshToken = newRefreshToken;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return newRefreshToken;
            }

            return refreshToken;
        }

        /// <summary>
        /// Refresh JWT
        /// </summary>
        /// <param name="refreshTokenString">Refresh token string</param>
        /// <returns>The task object containing an RefreshTokenResponse object</returns>
        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshTokenString)
        {
            // user with specified refresh token
            var user = await _context.Users.SingleOrDefaultAsync
                (u => u.RefreshToken.Token == refreshTokenString);

            if(user == null)
            {
                // token does not exist
                return new AuthenticationResult
                {
                    Errors = new [] { "Token does not exist" }
                };
            }

            var refreshToken = user.RefreshToken;

            if(refreshToken.IsExpired)
            {
                // token expired
                return new AuthenticationResult
                {
                    Errors = new[] { "Token expired" }
                };
            }

            // replace refresh token with the new one
            var newRefreshToken = CreateRefreshToken(user);
            refreshToken.Token = newRefreshToken.Token;
            refreshToken.Expires = newRefreshToken.Expires;
            _context.Update(refreshToken);
            _context.SaveChanges();

            // generate JWT
            var accessToken = await CreateTokenAsync(user);

            // send response
            return new AuthenticationResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Helper method for creating refresh token
        /// </summary>
        /// <param name="user">User entity</param>
        /// <returns>New refresh token object</returns>
        private RefreshToken CreateRefreshToken(ApplicationUser user)
        {
            var randomBytes = new byte[32];

            using(var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            var refreshToken = new RefreshToken()
            {
                Expires = DateTime.UtcNow.AddDays(7),
                Token = Convert.ToBase64String(randomBytes)
            };

            return refreshToken;
        }

        #endregion

        #region Roles

        public async Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user)
        {
            if(user == null)
            {
                return null;
            }

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> SetUserRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            try
            {
                // remove current roles
                var originalRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, originalRoles);

                // add new roles
                if(roles != null)
                {
                    await _userManager.AddToRolesAsync(user, roles);
                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
