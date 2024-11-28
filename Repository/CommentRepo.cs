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
            var comments = _context.Comments.AsQueryable();

            if (!String.IsNullOrWhiteSpace(query.Title))
            {
                comments = comments.Where(c => c.Title.Contains(query.Title));
            }

            if (query.CreatedOn.HasValue)
            {
                comments = comments.Where(c => c.CreatedOn.Date == query.CreatedOn.Value.Date);
            }

            switch (query.SortBy)
            {
                case commentSort.Id:
                    comments = query.IsDescending ? comments.OrderByDescending(c => c.Id)
                    : comments.OrderBy(c => c.Id);
                    break;
    
                case commentSort.Title:
                    comments = query.IsDescending ? comments.OrderByDescending(c => c.Title)
                    : comments.OrderBy(c => c.Title);
                    break;

                case commentSort.CreatedOn:
                    comments = query.IsDescending ? comments.OrderByDescending(c => c.CreatedOn)
                    : comments.OrderBy(c => c.CreatedOn);
                    break;
            }

            return await comments.ToListAsync();
        }

        public async Task<Comment?> GetyIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
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