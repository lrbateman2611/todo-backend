namespace Todo.Api.DTOs.Requests;

public record PostTodo
{
    public int? Id { get; set; } = default;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
