using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Zylex_Servers
{
    internal class Program
    {
        public static byte ServerType;
        public static byte GameEngineType;
        public static int Port;
        public static Dictionary<string, object> Settings = new Dictionary<string, object>();   
        public static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        static void Main(string[] args)
        {
            LoadServerSettings();
        }

        static void LoadServerSettings()
        {
            if (File.Exists(appPath + "StoredData/InstallerState.json"))
            {
                Console.WriteLine("Reading server state..");
                Settings = ApplicationUtils.ReadJsonFileToDictionary(appPath + "StoredData/InstallerState.json");
                Console.WriteLine("Loading Server Settings");
                ServerType = Byte.Parse(Settings["Server Type"].ToString());
                GameEngineType = Byte.Parse(Settings["Engine Type"].ToString());
                Port = int.Parse(Settings["Engine Type"].ToString());
                Console.WriteLine("Loaded");
                Console.Clear();
                OpenServer();
            }
            else
            {
                Installer.Install();
            }
        }

        static async void OpenServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine("Started server on: " + ApplicationUtils.GetPublicIpAddress() + ":" + Port);

            while (true)
            {
                // Accept an incoming TCP client connection
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected!");

                // Handle the client in a new task
                _ = Task.Run(() => HandleClientAsync(client));
            }
        }
        private static async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                try
                {
                    // Read data from the client
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        // Convert the bytes to a string
                        string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received: {clientMessage}");

                        // Prepare a response message
                        string responseMessage = $"Echo: {clientMessage}";
                        byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);

                        // Send the response back to the client
                        await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                        Console.WriteLine($"Sent: {responseMessage}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine("Client disconnected.");
                }
            }
        }


    }
}