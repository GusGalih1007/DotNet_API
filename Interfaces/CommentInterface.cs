using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface CommentInterface
    {
        Task<List<Comment>> GetAllAsync(QueryComment query);
        Task<Comment?> GetyIdAsync( int id );
        Task<Comment> CreateAsync(Comment commentModel);
        Task<Comment?> UpdateAsync( int id, UpdataCommentDto updataCommentDto);
        Task<Comment?> DeleteAsync( int id );
    }
}