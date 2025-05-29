using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WS_Socket.Client;

class Client
{
    /* Example of implementation:
    static async Task Main(string[] args)
    {
        Socket clientSocket = Connect();
        ListenToNetwork(clientSocket);
        Disconnect(clientSocket);
    }
    */

    //TODO: Implement as WPF
    //TODO: Implement actions sending, such as creating, deleting, running, pausing, and stopping jobs.
    private static Socket Connect()
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 5000);
        clientSocket.Connect(serverEndPoint);
        Console.WriteLine("Connected to the server.");
        return clientSocket;
    }

    private static void ListenToNetwork(Socket clientSocket)
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            Console.Write("Client: ");
            string message = Console.ReadLine();
            clientSocket.Send(Encoding.UTF8.GetBytes(message));

            if (message.ToLower() == "exit")
                break;

            int received = clientSocket.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine($"Server: {response}");
        }
    }

    private static void Disconnect(Socket socket)
    {
        socket.Close();
        Console.WriteLine("Connection closed.");
    }
}
