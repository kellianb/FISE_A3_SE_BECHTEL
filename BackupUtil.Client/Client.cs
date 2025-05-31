using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace WS_Socket.Client;

class Client
{
    private static List<Dictionary<string, string>> _jsonJobs;

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
            int received = clientSocket.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine($"Server: {response}");
            _jsonJobs = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(response);
        }
    }

    private static void Send(Socket socket, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        socket.Send(data);
        Console.WriteLine($"Sent: {message}");
    }

    private static void Disconnect(Socket socket)
    {
        socket.Close();
        Console.WriteLine("Connection closed.");
    }
}
