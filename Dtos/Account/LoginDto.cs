using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace api.Dtos.Account
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(l => l.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}