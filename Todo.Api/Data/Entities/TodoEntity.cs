using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Todo.Api.Data.Entities;

[Table("todo_items")]
public class TodoEntity : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("description")]
    public string Description { get; set; } = string.Empty;
    [Column("is_completed")]
    public bool IsCompleted { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("modified_at")]
    public DateTime ModifiedAt { get; set; }
}
