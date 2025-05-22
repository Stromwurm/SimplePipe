# SimplePipe

Simple Pipe is a small convenience wrapper for .NET's 'AnonymousPipeServerStream' and 'AnonymousPipeClientStream' classes. 
As it uses anonymous pipes, it can not do Network IPC and is intended for local use.

## How to use

1. Create an instance of the 'LocalServer' class.
2. With that instance, you need to get hold of the pipe handles to instatiate the 'LocalClient' class, using the 'GetPipeHandles' method.
3. Pass the pipe handles to the 'LocalClient' instance.
4. Use the 'DisposeLocalCopyOfClientHandle' method in the instance of 'LocalServer' once the 'LocalClient' instance has created the pipes with the handles.

NOTE:
  Dispoing of the handles in the server is necessary for IPC. If you, for some reason, use server and client in the same program, you do not want to call 'DisposeLocalCopyOfClientHandle' on the server instance as it will throw an exception      when you try to use sever and client.
  Disposing of the handles to early might cause problems, so you need to make sure you check if it safe to do so. A safe bet is to do so when the connection was established. As the server lives in another program, it is not possible to do it    automatically

5. Call 'Listen' method on each instances, or only one if you do not need to listen on both. Consider wrapping that call in a 'Task.Run(instance.Listen)' to make it run async.
6. Call 'Write' method to write to the other side.
7. Any messages received by either side will trigger a 'NewMessage' event on the respective side. In the server, the event data is a 'ClientMessage' instance, in the client it is a 'ServerMessage' instance.
