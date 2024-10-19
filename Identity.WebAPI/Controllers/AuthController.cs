using Identity.WebAPI.Dtos;
using Identity.WebAPI.Models;
using Identity.WebAPI.Options;
using Identity.WebAPI.Services;
using Identity.WebAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(UserManager<AppUser> _userManager,
        SignInManager<AppUser> _signInManager,
        IEMailSenderService _mailService,
        ITokenService _tokenService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Register(AppUserRegisterDto request, CancellationToken cancellationToken)
        {

            if (await _userManager.FindByEmailAsync(request.Email) is not null)
            {
                return BadRequest(new { Message = "This email address already taken" });
            }

            AppUser appUser = new AppUser()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };
            IdentityResult result = await _userManager.CreateAsync(appUser, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, UserRoles.User);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                //var url = Url.Action("ConfirmEmail", "Auth", new { userId = appUser.Id, token },Request.Scheme);
                var url = Url.Action("ConfirmEmail", "Auth", new { userId = appUser.Id, token });
                await _mailService.SendEmailAsync(appUser.Email, "Account Confirmation",
                $"Please click on the link to confirm your email account: <a href='https://localhost:7242{url}'>click here</a>");
                return Ok(new { Message = $"Registration completed successfully. Please click on the link to confirm your account: <a href='https://localhost:7242{url}'>click here</a>." });
            }
            else
            {
                return BadRequest(new { Message = result.Errors.SelectMany(x => x.Description) });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid User" });
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Email confirmed successfully" });
            }
            else
            {
                return BadRequest(new { Message = "Email confirmation failed" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            AppUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest(new { Message = "Email address not found" });
            }

            var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            string newPassword = PasswordGenerator.GeneratePassword(8);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, passwordToken, newPassword);
            if (result.Succeeded)
            {
                await _mailService.SendEmailAsync(user.Email!, "New Password", $"Your new password: {newPassword}");
                return Ok(new { Message = "Your password has been reset. Check your e-mail address." });
            }
            else
            {
                return BadRequest(result.Errors.Select(x => x.Description).ToList());
            }
        }


        [HttpPost]
        public async Task<IActionResult> LoginWithLockedOut(LoginRequestDto request, CancellationToken cancellationToken)
        {
            AppUser? user = _userManager.Users.FirstOrDefault(x => x.UserName == request.UserNameorEmail || x.Email == request.UserNameorEmail);
            if (user is null)
            {
                return BadRequest(new { Message = "User cannot find" });
            }
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

            if (result.IsLockedOut)
            {
                return StatusCode(500, "Your account has been locked for 5 minutes because you entered your password incorrectly 3 times.");
            }
            if (!result.Succeeded)
            {
                return StatusCode(500, "Password is wrong");
            }
            if (result.IsNotAllowed)
            {
                return StatusCode(500, "You must confirm your e-mail address.");
            }
            return Ok(new { Token = await _tokenService.GenerateToken(user) });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
        {
            AppUser? appUser = await _userManager.FindByEmailAsync(request.UserNameorEmail);

            if (appUser is null)
            {
                return BadRequest(new { Message = "User cannot find" });
            }
            bool passwordCheck = await _userManager.CheckPasswordAsync(appUser, request.Password);
            if (!passwordCheck)
            {
                return BadRequest(new { Message = "Password is wrong" });

            }
            return Ok(new { Token = await _tokenService.GenerateToken(appUser) });
        }
    }
}