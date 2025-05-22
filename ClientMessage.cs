namespace SimplePipe;

public record ClientMessage
{
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
    public string Message { get; init; }

    public ClientMessage(string message)
    {
        Message = message;
    }
}
