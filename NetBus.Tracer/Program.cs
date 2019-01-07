using NetBus.Logger;
using NetBus.Serializer;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetBus.Tracer
{
    class Program
    {


        static private async Task HandleTcpClient(ISerializer serializer, TcpClient tcpClient)
        {
            byte[] buffer = new byte[4096];
            byte[] message = null;

            using (var inStream = tcpClient.GetStream())
            using (var outStream = new MemoryStream())
            {
                while (true)
                {
                    int bytesRead = await inStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead <= 0)
                    {
                        message = outStream.GetBuffer();
                        break;
                    }
                    await outStream.WriteAsync(buffer, 0, bytesRead);
                }
            }

            tcpClient.Close();
            tcpClient.Dispose();

            var netBusLog = serializer.Deserialize<NetBusLog>(message);

            Console.WriteLine(netBusLog.ApplicationName);

        }

        static void Main(string[] args)
        {

            TcpListener listener = new TcpListener(IPAddress.Any, 30000);

            DefaultSerializer serializer = new DefaultSerializer();

            listener.Start();

            var task = Task.Run(async () =>
            {
                while (true)
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    HandleTcpClient(serializer, tcpClient);
                }
            });

            task.Wait();
            
        }
    }
}
