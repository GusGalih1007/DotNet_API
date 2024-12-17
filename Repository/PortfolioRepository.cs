using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();

            return portfolio;
        }

        public async Task<Portfolio?> DeleteAsync(Appuser appuser, string symbol)
        {
            var deletePortfolio = await _context.Portfolios.FirstOrDefaultAsync(x => x.AppUserId == appuser.Id && x.Stock.Symbol.ToLower() == symbol.ToLower());

            if (deletePortfolio == null)
            {
                return null;
            }

            _context.Portfolios.Remove(deletePortfolio);
            await _context.SaveChangesAsync();

            return deletePortfolio;
        }

        public async Task<List<Stock>> GetUserPortfolio(Appuser user)
        {
            return await _context.Portfolios.Where(u => u.AppUserId == user.Id)
            .Select(stock => new Stock
            {
                Id = stock.StockId,
                Symbol = stock.Stock.Symbol,
                CompanyName = stock.Stock.CompanyName,
                Purchase = stock.Stock.Purchase,
                LastDiv = stock.Stock.LastDiv,
                Industry = stock.Stock.Industry,
                MarketCap = stock.Stock.MarketCap
            }).ToListAsync();
        }
    }
}