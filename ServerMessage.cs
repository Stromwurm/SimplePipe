namespace SimplePipe;

public record ServerMessage
{
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
    public string Message { get; init; }

    public ServerMessage(string message)
    {
        Message = message;
    }
}
