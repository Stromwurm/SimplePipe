namespace SimplePipe;

public record ClientMessage
{
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
    public int MessageType { get; init; }
    public string Message { get; init; }

    public ClientMessage(string message)
    {
        Message = message[6..^1];
        MessageType = int.Parse(message[0..6]);

    }
}
