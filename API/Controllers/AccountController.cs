using System.Security.Claims;
using System.Text;
using API.DTOs;
using API.Services;
using Domain;
using Ifrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailSender _emailSender;


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, 
        TokenService tokenService, IConfiguration config, ILogger<AccountController> logger, EmailSender emailSender)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com")
            };
            _emailSender = emailSender;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email {Email}", loginDto.Email); // Make sure you have ILogger<AccountController> _logger injected

            var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == loginDto.Email);
            
            if(user == null) 
            {
                _logger.LogWarning("Login failed for email {Email}: User not found", loginDto.Email);
                return Unauthorized("Invalid email");
            }

            if(!user.EmailConfirmed)
            {
                return Unauthorized("Email not confirmed");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded)
            {
                _logger.LogWarning("Login failed for email {Email}: Incorrect password", loginDto.Email);
            }
            
            if(result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateUserObject(user);
            }

            return Unauthorized("Invalid password");
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email taken"); 
                return ValidationProblem();
            }

            if(await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                ModelState.AddModelError("username", "Username taken"); 
                return ValidationProblem();
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest("Problem registering user");

            var origin = Request.Headers["origin"];

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";

            var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>{verifyUrl}</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);

            return Ok("Registration successful - please verify your email address");
            
        }

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<ActionResult> VerifyEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null) return Unauthorized("Invalid email");

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(!result.Succeeded) return BadRequest("Could not confirm email");

            return Ok("Email confirmed - you can now login");
        }

        [AllowAnonymous]
        [HttpPost("forgotPassword")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null) return Unauthorized("Invalid email");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var origin = Request.Headers["origin"];

            var resetUrl = $"{origin}/account/resetPassword?token={token}&email={email}";

            var message = $"<p>Please click the below link to reset your password:</p><p><a href='{resetUrl}'>{resetUrl}</a></p>";

            await _emailSender.SendEmailAsync(email, "Reset Password", message);

            return Ok("Password reset email sent");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailConfirmation")]
        public async Task<ActionResult> ResendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null) return Unauthorized("Invalid email");

            if(user.EmailConfirmed) return BadRequest("Email already confirmed");

            var origin = Request.Headers["origin"];

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";

            var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>{verifyUrl}</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);

            return Ok("Email verification email sent");
        }

        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            await SetRefreshToken(user);
            return CreateUserObject(user);
        }

        [AllowAnonymous]
        [HttpPost("fbLogin")]
        public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        {
            var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:ApiSecret"];

            var verifyToken = await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

            if (!verifyToken.IsSuccessStatusCode)
            {
                return Unauthorized(new { Message = "Invalid Facebook token." });
            }

            var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

            var fbInfo = await _httpClient.GetFromJsonAsync<FacebookDto>(fbUrl);

            if (fbInfo == null)
            {
                return BadRequest(new { Message = "Failed to retrieve user information from Facebook." });
            }

            var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Email == fbInfo.Email);

            if(user != null) return CreateUserObject(user);

            user = new AppUser
            {
                DisplayName = fbInfo.Name,
                Email = fbInfo.Email,
                UserName = fbInfo.Email,
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        Id = "fb",
                        Url = fbInfo.Picture.Data.Url,
                        IsMain = true
                    }
                }
            };

            var result = await _userManager.CreateAsync(user);

            if(result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateUserObject(user);
            }

            return BadRequest("Problem registering user");

        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userManager.Users.Include(r => r.RefreshTokens).Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null) return Unauthorized();

            if (!user.RefreshTokens.Any(x => x.IsActive))
            {
                return Unauthorized();
            }

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (!oldToken.IsActive && oldToken != null)
            {
                return Unauthorized();
            }

            if(oldToken != null)
            {
                oldToken.Revoked = DateTime.UtcNow;
            }


            return CreateUserObject(user);
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GetRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);


        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}