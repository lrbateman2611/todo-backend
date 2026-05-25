namespace Todo.Api.Models;

public record Auth0Secret
{
    public required string Domain { get; set; }
    public required string ClientId { get; set; }
    public required string Audience { get; set; }
}
