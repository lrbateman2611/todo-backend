using Todo.Api.Data.Entities;
using Todo.Api.DTOs.Requests;

namespace Todo.Api.Models;

public class TodoItem
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public TodoItem(TodoEntity todoEntity)
    {
        Id = todoEntity.Id;
        Title = todoEntity.Name;
        Description = todoEntity.Description;
        IsCompleted = todoEntity.IsCompleted;
        Category = todoEntity.Category;
        Color = todoEntity.Color;
    }

    public TodoItem(PostTodo todoRequest)
    {
        Id = todoRequest.Id;
        Title = todoRequest.Title;
        Description = todoRequest.Description;
        IsCompleted = todoRequest.IsCompleted;
        Category = todoRequest.Category;
        Color = todoRequest.Color;
    }

    public TodoItem(UpdateTodo todoRequest)
    {
        Id = todoRequest.Id;
        Title = todoRequest.Title;
        Description = todoRequest.Description;
        IsCompleted = todoRequest.IsCompleted;
        Category = todoRequest.Category;
        Color = todoRequest.Color;
    }
}

