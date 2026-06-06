using Todo.Api.Data.Entities;
using Todo.Api.Models;

namespace Todo.Api.Data;

public interface ITodoData
{
    Task<IEnumerable<TodoEntity>> GetTodoItemsForUser(string userId);
    Task<bool> AddTodoItem(TodoItem item, string userId);
    Task<bool> UpdateTodoItem(TodoItem item, string userId);
    Task<bool> DeleteTodoItem(int todoId, string userId);
}

public class TodoData(Supabase.Client supabaseClient): ITodoData
{
    public async Task<IEnumerable<TodoEntity>> GetTodoItemsForUser(string userId)
    {
        var result = await supabaseClient.From<TodoEntity>()
            .Select("*, todo_mapping!inner(*)")
            .Filter("todo_mapping.user_id", Supabase.Postgrest.Constants.Operator.Equals, userId)
            .Get();

        return result.Models;
    }

    public async Task<bool> AddTodoItem(TodoItem item, string userId)
    {
        var parameters = new Dictionary<string, object>
        {
            {"p_description", item.Description },
            {"p_name", item.Title },
            {"p_category", item.Category },
            {"p_color", item.Color },
            {"p_user_id", userId }
        };

        var result = await supabaseClient.Rpc("insert_todo", parameters);

        return result.ResponseMessage?.IsSuccessStatusCode == true;
    }

    public async Task<bool> UpdateTodoItem(TodoItem item, string userId)
    {
        if (item.Id == null)
        {
            return false;
        }

        var parameters = new Dictionary<string, object>
        {
            {"p_todo_id", item.Id.Value },
            {"p_name", item.Title },
            {"p_description", item.Description },
            {"p_is_completed", item.IsCompleted },
            {"p_category", item.Category },
            {"p_color", item.Color },
            {"p_user_id", userId }
        };

        var result = await supabaseClient.Rpc("update_todo", parameters);

        return result.ResponseMessage?.IsSuccessStatusCode == true;
    }

    public async Task<bool> DeleteTodoItem(int todoId, string userId)
    {
        var parameters = new Dictionary<string, object>
        {
            {"p_todo_id", todoId },
            {"p_user_id", userId }
        };

        var result = await supabaseClient.Rpc("delete_todo", parameters);

        return result.ResponseMessage?.IsSuccessStatusCode == true;
    }
}
