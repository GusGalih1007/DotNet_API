using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepo : CommentInterface
    {
        private readonly ApplicationDBContext _context;
        public CommentRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(d => d.Id == id);
            if (commentModel == null)
            {
                return null;
            }
            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();

            return commentModel;
        }

        public async Task<List<Comment>> GetAllAsync(QueryComment query)
        {
            var comments = _context.Comments.Include(a => a.AppUser).AsQueryable();

            if (!String.IsNullOrWhiteSpace(query.Title))
            {
                comments = comments.Where(c => c.Title.Contains(query.Title));
            }

            if (!String.IsNullOrWhiteSpace(query.Symbol))
            {
                comments = comments.Where(s => s.Stock.Symbol == query.Symbol);
            }

            if (query.IsDescending == true)
            {
                comments = comments.OrderByDescending(c => c.CreatedOn);
            }

            return await comments.ToListAsync();
        }

        public async Task<Comment?> GetyIdAsync(int id)
        {
            return await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment?> UpdateAsync(int id, UpdataCommentDto updataCommentDto)
        {
            var editStock = await _context.Comments.FirstOrDefaultAsync(u => u.Id == id);
            if (editStock == null)
            {
                return null;
            }
            editStock.Title = updataCommentDto.Title;
            editStock.Content = updataCommentDto.Content;

            await _context.SaveChangesAsync();

            return editStock;
        }
    }
}