namespace ToDoList.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.DTOs;
using ToDoList.Domain.Models;
using ToDoList.Persistence.Repositories;

[ApiController]
[Route("api/[Controller]")]
public class ToDoItemsController : ControllerBase
{
    private readonly IRepositoryAsync<ToDoItem> repository;

    public ToDoItemsController(IRepositoryAsync<ToDoItem> repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<ToDoItemGetResponseDto>> Create(ToDoItemCreateRequestDto request)
    {
        var item = request.ToDomain();
        try
        {
            await repository.Create(item);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
        }

        return CreatedAtAction(
            nameof(ReadById),
            new { toDoItemId = item.ToDoItemId },
            ToDoItemGetResponseDto.FromDomain(item));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoItemGetResponseDto>>> Read()
    {
        try
        {
            var itemsToSendList = await repository.Read();

            return (itemsToSendList == null)
            ? NotFound()
            : Ok(itemsToSendList.Select(ToDoItemGetResponseDto.FromDomain));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{toDoItemId:int}")]
    public async Task<ActionResult<ToDoItemGetResponseDto>> ReadById(int toDoItemId)
    {
        try
        {
            var item = await repository.ReadById(toDoItemId);

            return item == null
            ? NotFound()
            : Ok(ToDoItemGetResponseDto.FromDomain(item));
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{toDoItemId:int}")]
    public async Task<IActionResult> UpdateById(int toDoItemId, [FromBody] ToDoItemUpdateRequestDto request)
    {
        try
        {
            var updatedItem = new ToDoItem
            {
                Name = request.Name,
                Description = request.Description,
                IsCompleted = request.IsCompleted
            };

            var item = await repository.UpdateById(toDoItemId, updatedItem);
            if (item == null) //cely return by sel udelat na jeden radek - stejne jak to mas v ReadById
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{toDoItemId:int}")]
    public async Task<IActionResult> DeleteById(int toDoItemId)
    {
        try
        {
            var item = await repository.DeleteById(toDoItemId);
            if (item == null) //cely return by sel udelat na jeden radek - stejne jak to mas v ReadById
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
        }

    }
}
