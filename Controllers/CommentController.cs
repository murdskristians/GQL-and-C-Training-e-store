using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Mappers;
using api.Dtos.Comment;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comments = await _commentRepo.GetAllAsync();

            var commentDtos = comments.Select(s => s.ToCommentDto());

            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var commentDto = comment.ToCommentDto();
            return Ok(commentDto);
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, [FromBody] CreateCommentDto commentDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!await _stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock does not exist");
            }

            var commentModel = commentDto.ToCommentFromCreate(stockId);
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
                        if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());
            if(comment == null)
            {
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
                        if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var commentModel = await _commentRepo.DeleteAsync(id);
            if(commentModel == null)
            {
                return NotFound("Comment not found");
            }

            return Ok(commentModel);
        }
    }
}