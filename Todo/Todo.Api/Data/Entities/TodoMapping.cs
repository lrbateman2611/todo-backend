using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Todo.Api.Data.Entities;

[Table("todo_mapping")]
public class TodoMapping : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;
    [Reference(typeof(TodoEntity), foreignKey: "todo_id")]
    public int TodoId { get; set; }
}
