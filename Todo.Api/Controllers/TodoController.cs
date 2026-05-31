using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.DTOs.Requests;
using Todo.Api.Models;
using Todo.Api.Services;

namespace Todo.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<TodoItem>> Get()
    {
        return todoService.GetTodoItems();
    }

    [HttpPost]
    public Task<Boolean> Post([FromBody] PostTodo request)
    {
        return todoService.AddTodoItem(new TodoItem(request));
    }

    [HttpPut("{id}")]
    public Task<Boolean> Put(int id, [FromBody] UpdateTodo request)
    {
        if (id != request.Id)
        {
            return Task.FromResult(false);
        }
        return todoService.UpdateTodoItem(new TodoItem(request));
    }

    [HttpDelete("{id}")]
    public Task<Boolean> Delete(int id)
    {
        return todoService.DeleteTodoItem(id);
    }
}
