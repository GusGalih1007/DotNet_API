using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using FluentValidation;

namespace api.Validations
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Title is Required")
                .Length(2, 20).WithMessage("Title must be between 2 to 20 characters");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Content is required");
        }
    }
}