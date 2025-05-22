using System.IO.Pipes;
using System.Text;

namespace SimplePipe;

public class LocalServer : IDisposable
{
    private AnonymousPipeServerStream _pipeOut;
    private AnonymousPipeServerStream _pipeIn;
    private StreamReader _reader;
    private StreamWriter _writer;

    private CancellationTokenSource? _cts;

    public bool IsConnected => _pipeOut.IsConnected && _pipeIn.IsConnected;

    public event EventHandler<ClientMessage> NewMessage;
    public event EventHandler<string> ServerMessage;

    public LocalServer()
    {
        _pipeOut = new(PipeDirection.Out, HandleInheritability.Inheritable);
        _writer = new(_pipeOut, Encoding.UTF8);

        _pipeIn = new(PipeDirection.In, HandleInheritability.Inheritable);
        _reader = new(_pipeIn, Encoding.UTF8);
    }

    public void Listen()
    {
        if (!IsConnected)
            throw new InvalidOperationException("Client is not connected!");

        _cts = new();

        ServerMessage?.Invoke(this, "Server listening...");
        
        while (!_cts.Token.IsCancellationRequested)
        {
            Thread.Sleep(25);

            try
            {
                
                var msg = _reader.ReadLine();

                if (msg is null)
                    continue;

                ClientMessage cMsg = new(msg);
                NewMessage?.Invoke(this, cMsg);

            }
            catch(Exception ex)
            {
                ServerMessage?.Invoke(this, $"SERVER {ex.Message}");
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
        _pipeIn.Dispose();
    }

    /// <summary>
    /// Watch out when assigning the handles to the client. The PipeIn from the server needs to be assigned to PipeOut from the client and vice versa.
    /// </summary>
    /// <returns>
    ///     <list type="number">
    ///         <item>PipeIn</item>
    ///         <item>PipeOut</item>
    ///     </list>
    /// </returns>
    public string[] GetPipeHandles()
    {
        string[] handles = [_pipeIn.GetClientHandleAsString(), _pipeOut.GetClientHandleAsString()];
        return handles;
    }

    public void DisposeLocalCopyOfClientHandle()
    {
        _pipeIn.DisposeLocalCopyOfClientHandle();
        _pipeOut.DisposeLocalCopyOfClientHandle();
    }

    public LocalClient GetClient()
    {
        LocalClient lc = new(_pipeOut.GetClientHandleAsString(), _pipeIn.GetClientHandleAsString());
        lc.HandleDisposeReady += Handle_Lc_HandleDisposeReady;
        

        return lc;
    }

    private void Handle_Lc_HandleDisposeReady(object? sender, EventArgs e)
    {
        _pipeOut.DisposeLocalCopyOfClientHandle();
        _pipeIn.DisposeLocalCopyOfClientHandle();

        (sender as LocalClient).HandleDisposeReady -= Handle_Lc_HandleDisposeReady;
    }
}
