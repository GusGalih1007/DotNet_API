using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                StockId = commentModel.StockId
            };
        }
        public static Comment CreateCommentDto(this CreateCommentDto createComment, int stockId)
        {
            return new Comment
            {
                Title = createComment.Title,
                Content = createComment.Content,
                StockId = stockId
            };
        }
        
        public static Comment UpdateCommentDto(this UpdataCommentDto updataComment, int stockId)
        {
            return new Comment
            {
                Title = updataComment.Title,
                Content = updataComment.Content,
                StockId = stockId
            };
        }
    }
}