using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using LM_Exchange.Custom_Exception;
using LM_Exchange.Data;
using LM_Exchange.Dtos;
using LM_Exchange.Model;
using LM_Exchange.Models.Responses;
using LM_Exchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LM_Exchange.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Logincontroller : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILoginService _tokenService;

        public Logincontroller(AppDbContext context, ILoginService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

    
        [HttpPost("/Register")]
        public async Task<IActionResult> Register([FromBody] AddRequest request)
        {
            try { 
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                    throw new EmailAlreadyRegistred("User already Registered");
             }
            var refreshToken = _tokenService.GenerateRefreshToken();

            var user = new User
            {
                Role = request.Role,
                Email = request.Email,
                Password = request.Password,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7)
            };
                _tokenService.ValidateUser(user);
                _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var res = new ApiResponse<object>
            {
                data = new
                {
                    id = user.Id,
                    message = "User successfully registered"
                }
            };
            return Ok(res);
            }
            
            catch (InvalidEmail ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (EmailAlreadyRegistred ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidPassword ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidRole ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to Register User",
                        code = "500"
                    }
                });
            }
        }


     
        [HttpPost("/Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                if (string.IsNullOrWhiteSpace(request.Email) || !emailRegex.IsMatch(request.Email))
                {
                    throw new InvalidEmail("Invalid email format.");

                }

                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                {
                    throw new InvalidPassword("Password must be at least 6 characters long.");
                }
                // Check if user exists
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Email == request.Email && u.Password == request.Password);

                if (user == null)
                    throw new InvalidEmailOrPassword("InvalidEmailOrPassword");

                // Create access and refresh tokens
                var tokenResponse = _tokenService.CreateToken(user);

                // Update refresh token in DB
                user.RefreshToken = tokenResponse.RefreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        tokenResponse.AccessToken,
                        tokenResponse.RefreshToken
                    }
                };
                return Ok(res);
            }

            catch (InvalidEmail ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidPassword ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidEmailOrPassword ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
        }

        [Authorize]
        [HttpPost("/GenerateAcessTokenFromRefreshToken")]
        public async Task<IActionResult> Refresh([FromBody] LoginResponseDto tokenModel)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);

            var email = principal.FindFirstValue(ClaimTypes.Email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            var newToken = _tokenService.CreateToken(user);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var res = new ApiResponse<object>
            {
                data = new
                {
                    newToken.AccessToken,
                    newToken.RefreshToken
                }
            };
            return Ok(res);
           
        }



    }

}
