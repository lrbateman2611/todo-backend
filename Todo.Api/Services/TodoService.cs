using Todo.Api.Data;
using Todo.Api.Models;

namespace Todo.Api.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetTodoItems();
    Task<bool> AddTodoItem(TodoItem item);
    Task<bool> UpdateTodoItem(TodoItem item);
    Task<bool> DeleteTodoItem(int todoId);
}

public class TodoService(ITodoData todoData, IUserService userService) : ITodoService
{
    public async Task<IEnumerable<TodoItem>> GetTodoItems()
    {
        var userId = userService.GetCurrentUserId();
        var todoEntities = await todoData.GetTodoItemsForUser(userId);
        return todoEntities.Select(x => new TodoItem(x));
    }

    public async Task<bool> AddTodoItem(TodoItem item)
    {
        var userId = userService.GetCurrentUserId();
        return await todoData.AddTodoItem(item, userId);
    }

    public async Task<bool> UpdateTodoItem(TodoItem item)
    {
        var userId = userService.GetCurrentUserId();
        return await todoData.UpdateTodoItem(item, userId);
    }

    public async Task<bool> DeleteTodoItem(int todoId)
    {
        var userId = userService.GetCurrentUserId();
        return await todoData.DeleteTodoItem(todoId, userId);
    }
}
