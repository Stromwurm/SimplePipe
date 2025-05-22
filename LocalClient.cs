using System.IO.Pipes;
using System.Text;

namespace SimplePipe;

public class LocalClient : IDisposable
{
    private AnonymousPipeClientStream _pipeOut;
    private AnonymousPipeClientStream _pipeIn;
    private StreamReader _reader;
    private StreamWriter _writer;

    private CancellationTokenSource? _cts;

    public bool IsConnected => _pipeOut.IsConnected && _pipeIn.IsConnected;

    public event EventHandler<ServerMessage> NewMessage;
    public event EventHandler<string> ClientMessage;
    public event EventHandler HandleDisposeReady;

    public LocalClient(string handleIn, string handleOut)
    {
        _pipeIn = new(PipeDirection.In, handleIn); 
        _reader = new(_pipeIn, Encoding.UTF8);

        _pipeOut = new(PipeDirection.Out, handleOut);
        _writer = new(_pipeOut, Encoding.UTF8);
    }

    public void Listen()
    {
        HandleDisposeReady?.Invoke(this, new());
        if (!IsConnected)
            throw new InvalidOperationException("Client is not connected!");

        _cts = new();

        ClientMessage?.Invoke(this, "Client listening...");

        while (!_cts.Token.IsCancellationRequested)
        {
            Thread.Sleep(25);

            try
            {
                var msg = _reader.ReadLine();

                if (msg is null)
                    continue;

                ServerMessage cMsg = new(msg);
                NewMessage?.Invoke(this, cMsg);

            }
            catch (Exception ex)
            {
                ClientMessage?.Invoke(this, $"SERVER {ex.Message}");
            }
        }
    }

    public void Write(string msg)
    {
        if (!IsConnected)
            throw new InvalidOperationException("No client connected!");

        _writer.WriteLine(msg);
        _writer.Flush();
    }

    public void Dispose()
    {
        _pipeOut.Dispose();
    }
}