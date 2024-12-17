using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Azure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel == null)
            {
                return null;
            }
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryStock query)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.Industry))
            {
                stocks = stocks.Where(s => s.Industry.Contains(query.Industry));
            }

            switch (query.SortBy)
            {

                case Item.Id:
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Id)
                    : stocks.OrderBy(s => s.Id);
                    break;

                case Item.Symbol:
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol)
                    : stocks.OrderBy(s => s.Symbol);
                    break;

                case Item.CompanyName:
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.CompanyName)
                    : stocks.OrderBy(s => s.CompanyName);
                    break;

                case Item.Purchase:
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Purchase)
                    : stocks.OrderBy(s => s.Purchase);
                    break;

                case Item.LastDiv:
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.LastDiv)
                    : stocks.OrderBy(s => s.LastDiv);
                    break;

                case Item.Industry:
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Industry)
                    : stocks.OrderBy(s => s.Industry);
                    break;

                case Item.MarketCap:
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.MarketCap)
                    : stocks.OrderBy(s => s.MarketCap);
                    break;
            }

            return await stocks.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockDto updateDto)
        {
            var editStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (editStock == null)
            {
                return null;
            }

            editStock.Symbol = updateDto.Symbol;
            editStock.CompanyName = updateDto.CompanyName;
            editStock.Purchase = updateDto.Purchase;
            editStock.LastDiv = updateDto.LastDiv;
            editStock.Industry = updateDto.Industry;
            editStock.MarketCap = updateDto.MarketCap;

            await _context.SaveChangesAsync();

            return editStock;
        }
    }
}