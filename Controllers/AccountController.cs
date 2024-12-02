using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Appuser> _userManager;
        private readonly IValidator<RegisterDto> _regisValidator;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<Appuser> _signInManager;
        private readonly IValidator<LoginDto> _loginValidate;
        public AccountController(
            UserManager<Appuser> userManager,
            SignInManager<Appuser> signInManager,
            IValidator<RegisterDto> regisValidator,
            IValidator<LoginDto> loginValidate,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _regisValidator = regisValidator;
            _tokenService = tokenService;
            _loginValidate = loginValidate;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var validateRegister = await _regisValidator.ValidateAsync(registerDto);
                if (!validateRegister.IsValid)
                {
                    validateRegister.AddToModelState(this.ModelState);

                    return BadRequest(ModelState);
                }

                var appUser = new Appuser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser)
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createUser.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        //login 

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var validateResult = await _loginValidate.ValidateAsync(loginDto);
            if (!validateResult.IsValid)
            {
                validateResult.AddToModelState(this.ModelState);

                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(l => l.UserName == loginDto.Username || l.Email == loginDto.Username);

            if (user == null)
            {
                return Unauthorized("Invalid Username/Email!");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid Password!");
            }

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }
    }
}