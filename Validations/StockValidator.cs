using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using FluentValidation;

namespace api.Validations
{
    public class StockValidator : AbstractValidator<Stock>
    {
        public StockValidator()
        {
            RuleFor(s => s.Symbol)
                .NotEmpty().WithMessage("Symbol is required.")
                .Length(1, 10).WithMessage("Symbol must be beween 1 to 10 characters");

            RuleFor(s => s.CompanyName)
                .NotEmpty().WithMessage("Company Name is required.")
                .Length(2, 35).WithMessage("Company name must be beween 1 to 10 characters");

            RuleFor(s => s.Purchase)
                .NotEmpty().WithMessage("Purchase is required.")
                .PrecisionScale(18, 2, false);

            RuleFor(s => s.LastDiv)
                .NotEmpty().WithMessage("Last Divition is required.")
                .PrecisionScale(18, 2, false)
                .LessThanOrEqualTo(100);

            RuleFor(s => s.Industry)
                .NotEmpty().WithMessage("Industry is required.")
                .Length(2, 20).WithMessage("Industry must be between 2 to 20 character");

            RuleFor(s => s.MarketCap)
                .NotEmpty();
        }
    }
}