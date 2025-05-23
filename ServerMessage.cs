namespace SimplePipe;

public record ServerMessage
{
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
    public int MessageType { get; init; }
    public string Message { get; init; }

    public ServerMessage(string message)
    {
        Message = message[6..^1];
        MessageType = int.Parse(message[0..6]);

    }
}
