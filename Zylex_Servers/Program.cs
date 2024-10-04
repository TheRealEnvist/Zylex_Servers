using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.CompilerServices;

namespace Zylex_Servers
{
    internal class Program
    {
        public static byte ServerType;
        public static byte GameEngineType;
        public static byte ConnectionMethod;
        public static int Port;
        public static Dictionary<string, object> Settings = new Dictionary<string, object>();
        public static List<TcpClient> Clients = new List<TcpClient>();
        public static Dictionary<TcpClient, int> ConnectionIDs = new Dictionary<TcpClient, int>();
        public static Dictionary<int, Dictionary<string, string>> ObjectsModified = new Dictionary<int, Dictionary<string,string>>();
        public static TcpListener listener;
        public static TcpClient MasterClient;
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
                ConnectionMethod = Byte.Parse(Settings["Connection Type"].ToString());
                Port = int.Parse(Settings["Port"].ToString());
                Console.WriteLine("Loaded");
                Console.Clear();
                OpenServer();
                Console.WriteLine("Press any key to close the server..");
                Console.ReadKey();
                CloseServer();
                Console.WriteLine("Press any key to close the window..");
                Console.ReadKey();
            }
            else
            {
                Installer.Install();
            }
        }

        public static async void OpenServer()
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine("Started server on: " + ApplicationUtils.GetPublicIpAddress() + ":" + Port);

            while (true)
            {
                // Accept an incoming TCP client connection
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected!");
                Clients.Add(client);
                ConnectionIDs.Add(client, GenerateUniqueConnectionID(ConnectionIDs.Values.ToList<int>()));

                string responseMessage = BuildServerSyncLoad(ConnectionIDs[client]);
                byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);

                await client.GetStream().WriteAsync(responseBuffer, 0, responseBuffer.Length);

                // Handle the client in a new task
                _ = Task.Run(() => HandleClientAsync(client));
            }

            

        }

        static private int GenerateUniqueConnectionID(List<int> existingIDs)
        {
            int connectionID;
            Random random = new Random();

            do
            {
                connectionID = random.Next(); 
            }
            while (existingIDs.Contains(connectionID)); 

            return connectionID;
        }


        private static string BuildServerSyncLoad(int ConnectionID)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2["type"] = "ServerSync";

            foreach (int i in ObjectsModified.Keys)
            {
                dict[i] = ApplicationUtils.DictionaryToJson(ObjectsModified[i]);
            }
            dict2["value"] = dict;
            dict2["ConnectionID"] = ConnectionID;
            return ApplicationUtils.DictionaryToJson(dict2).ToString();
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
                        if(ApplicationUtils.IsValidJson(clientMessage))
                        {
                            Console.WriteLine("Received JSON");
                            Dictionary<string, object> json = ApplicationUtils.JsonStringToDictionary(clientMessage);
                            if(ConnectionMethod == 4)
                            {
                                if (json.ContainsKey("type"))
                                {
                                    if (json["type"].ToString() == "Packet")
                                    {
                                        SendToOtherClients(clientMessage, client);
                                        return;
                                    }
                                }
                                else
                                {
                                    if (ObjectsModified.ContainsKey(int.Parse(json["instanceID"].ToString())))
                                    {
                                        ObjectsModified[int.Parse(json["instanceID"].ToString())][json["type"].ToString()] = json["value"].ToString();
                                    }
                                    else
                                    {
                                        ObjectsModified[int.Parse(json["instanceID"].ToString())] = new Dictionary<string, string>();
                                        ObjectsModified[int.Parse(json["instanceID"].ToString())][json["type"].ToString()] = json["value"].ToString();
                                    }
                                }
                                
                            }
                        }

                        // Prepare a response message
                        string responseMessage = $"{clientMessage}";
                        byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);

                        // Send the response back to the client
                        if(ConnectionMethod == 4)
                        {
                            SendToAllClients(responseMessage);
                        }
                        else
                        {
                            await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
                        }
                        
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
                    Clients.Remove(client);
                    ConnectionIDs.Remove(client);
                }
            }
        }

        public static async void SendToAllClients(string message)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes(message);
            foreach (TcpClient i in Clients)
            {
                await i.GetStream().WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
        }

        public static async void SendToOtherClients(string message, TcpClient client)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes(message);
            foreach (TcpClient i in Clients)
            {
                if(i != client)
                {
                    await i.GetStream().WriteAsync(responseBuffer, 0, responseBuffer.Length);
                }
            }
        }
        public static async void CloseServer()
        {
            listener.Stop();
        }


    }
}