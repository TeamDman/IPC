using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNamedPipeServer
{
    class Program
    {
        private const string PipeName = @"testpipe";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Named Pipe Server...");
            await using var server = new NamedPipeServerStream(PipeName, PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

            Console.WriteLine("Waiting for client to connect...");
            await server.WaitForConnectionAsync();
            Console.WriteLine("Client connected!");

            // Write Fibonacci sequence to the client
            int a = 0, b = 1;

            while (true)
            {
                int fib = a;
                a = b;
                b = a + fib;

                // Write the number to the client
                byte[] messageBytes = Encoding.UTF8.GetBytes(fib + "\n");
                await server.WriteAsync(messageBytes, 0, messageBytes.Length);
                await server.FlushAsync();

                Console.WriteLine($"Sent: {fib}");

                // Wait before sending the next number
                await Task.Delay(1000);
            }
        }
    }
}
