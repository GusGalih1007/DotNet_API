using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extension;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly CommentInterface _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly IValidator<Comment> _validator;
        private readonly UserManager<Appuser> _userManager;
        private readonly IFMPService _fmpService;
        public CommentController(
            CommentInterface commentRepo,
            IStockRepository stockRepo,
            IValidator<Comment> validator,
            UserManager<Appuser> userManager,
            IFMPService fMPService)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
            _validator = validator;
            _fmpService = fMPService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryComment query)
        {
            var comments = await _commentRepo.GetAllAsync(query);

            if (comments == null)
            {
                return NotFound("No Commment");
            }

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepo.GetyIdAsync(id);

            if (comment == null)
            {
                return NotFound("Comment Not Found");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{symbol:alpha}")]
        public async Task<IActionResult> Create([FromRoute] string symbol, [FromForm] CreateCommentDto createComment)
        {
            

            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if(stock == null)
                {
                    return BadRequest("Stock does no exist");
                }
                else
                {
                    await _stockRepo.CreateAsync(stock);
                }
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var commentModel = createComment.CreateCommentDto(stock.Id);

            commentModel.AppUserId = appUser.Id;

            await _commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdataCommentDto updataComment)
        {

            var validationResult = await _validator.ValidateAsync(updataComment.UpdateCommentDto(id));

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);

                return BadRequest(ModelState);
            }

            var commentModel = await _commentRepo.UpdateAsync(id, updataComment);

            if (commentModel == null)
            {
                return NotFound("Comment Not Found");
            }

            return Ok(commentModel.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var delComment = await _commentRepo.DeleteAsync(id);

            if (delComment == null)
            {
                return NotFound("Comment Not Found");
            }

            return Ok(delComment);
        }
    }
}