using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace BackupUtil.Server;

class Server
{
    /*Example of usage:
    static void Main(string[] args)
    {
        Socket serverSocket = Connect();
        Socket clientSocket = AcceptConnection(serverSocket);
        ListenToNetwork(clientSocket);
        Disconnect(clientSocket);
        Disconnect(serverSocket);
    }
    */

    //TODO: Implement as WPF
    //TODO: Implement job management logic, such as creating, deleting, running, pausing, and stopping jobs.
    private static Socket Connect()
    {
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5000);
        serverSocket.Bind(endPoint);
        serverSocket.Listen(1);
        Console.WriteLine("Server is listening on port 5000...");
        return serverSocket;
    }

    private static Socket AcceptConnection(Socket serverSocket, string jobs)
    {
        Socket clientSocket = serverSocket.Accept();
        IPEndPoint clientEndPoint = clientSocket.RemoteEndPoint as IPEndPoint;
        Console.WriteLine($"Client connected: {clientEndPoint.Address}:{clientEndPoint.Port}");
        clientSocket.Send(Encoding.UTF8.GetBytes(jobs));
        return clientSocket;
    }


    private static void ListenToNetwork(Socket clientSocket)
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            int received = clientSocket.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine($"Client: {message}");
            Dictionary<string, string> jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
            switch (jsonResponse["action"])
            {
                case "create":
                // Handle create action
                Console.WriteLine("Creating job...");
                break;
                case "delete":
                // Handle delete action
                Console.WriteLine("Deleting job...");
                break;
                case"play":
                // Handle run action
                Console.WriteLine("Running job...");
                break;
                case "pause":
                // Handle pause action
                Console.WriteLine("Pausing job...");
                break;
                case "stop":
                // Handle stop action
                Console.WriteLine("Stopping job...");
                break;
            }

            if (message.ToLower() == "exit")
                break;

            Console.Write("Server: ");
            string response = Console.ReadLine();
            clientSocket.Send(Encoding.UTF8.GetBytes(response));
        }
    }

    private static void Disconnect(Socket socket)
    {
        socket.Close();
        Console.WriteLine("Connection closed.");
    }

}
