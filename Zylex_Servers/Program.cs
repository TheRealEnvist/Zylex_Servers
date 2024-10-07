using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;

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
        public static Dictionary<TcpClient, NetworkStream> ClientStream = new Dictionary<TcpClient, NetworkStream>();
        public static Dictionary<TcpClient, int> ConnectionIDs = new Dictionary<TcpClient, int>();
        public static Dictionary<int, Dictionary<string, string>> ObjectsModified = new Dictionary<int, Dictionary<string, string>>();
        public static TcpListener listener;
        public static Socket ServerSocket;
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
            ServerSocket = listener.Server;

            while (true)
            {
                // Accept an incoming TCP client connection
                TcpClient client = await listener.AcceptTcpClientAsync();
                Clients.Add(client);
                ClientStream.Add(client, client.GetStream());
                ConnectionIDs.Add(client, GenerateUniqueConnectionID(ConnectionIDs.Values.ToList<int>()));
                Console.WriteLine("Client connected with connection id " + ConnectionIDs[client]);


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
                byte[] lengthBuffer = new byte[4]; // Buffer to hold the length prefix (4 bytes for an integer)
                byte[] messageBuffer;
                int bytesRead;

                try
                {
                    while (true)
                    {


                        bytesRead = await stream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
                        if (bytesRead != 4)
                            throw new Exception("Failed to read message length prefix. Client may have disconnected.");

                        // Convert lengthBuffer to an integer to get the total message length
                        int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                        // 2. Allocate buffer based on the received message length
                        messageBuffer = new byte[messageLength];
                        int totalBytesRead = 0;

                        // 3. Read until the entire message is received
                        while (totalBytesRead < messageLength)
                        {
                            bytesRead = await stream.ReadAsync(messageBuffer, totalBytesRead, messageLength - totalBytesRead);
                            if (bytesRead == 0)
                                throw new Exception("Client disconnected before the full message was received.");

                            totalBytesRead += bytesRead;
                        }

                        // 4. At this point, the entire message is in `messageBuffer`
                        string base64Message = Encoding.UTF8.GetString(messageBuffer);
                        string clientMessage = ApplicationUtils.DecodeFromBase64(base64Message);
                        Console.WriteLine($"Full Base64 String Received: {base64Message}");
                        Console.WriteLine($"Decoded Message: {clientMessage}");
                        //Console.WriteLine($"Received: {clientMessage}");
                        Dictionary<string, object> json = new Dictionary<string, object>();
                        try
                        {
                            json = ApplicationUtils.JsonStringToDictionary(clientMessage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Couldnt parse JSON, trying for a packet");
                            return;
                        }
                        Console.WriteLine("Received JSON");

                        if (ConnectionMethod == 4)
                        {
                            if (json.ContainsKey("type"))
                            {
                                if (json["type"].ToString() == "Packet")
                                {
                                    Console.WriteLine("Recived Packet | Decoding on server side..");
                                    Dictionary<string, object> dict = ApplicationUtils.JsonStringToDictionary(clientMessage);
                                    Console.WriteLine("1");
                                    Console.WriteLine(dict["value"].ToString());
                                    Dictionary<int, Dictionary<string, object>> Packet = DecodePacket(dict["value"].ToString());

                                    Console.WriteLine("2");
                                    foreach (Dictionary<string, object> i in Packet.Values)
                                    {
                                        if (ObjectsModified.ContainsKey(int.Parse(i["instanceID"].ToString())))
                                        {
                                            ObjectsModified[int.Parse(i["instanceID"].ToString())][i["type"].ToString()] = i["value"].ToString();
                                        }
                                        else
                                        {
                                            ObjectsModified[int.Parse(i["instanceID"].ToString())] = new Dictionary<string, string>();
                                            ObjectsModified[int.Parse(i["instanceID"].ToString())][i["type"].ToString()] = i["value"].ToString();
                                        }
                                    }
                                    Console.WriteLine("Updated server status!");
                                    SendToOtherClients(clientMessage, client);
                                    Console.WriteLine("Packet sent to other clients..");

                                    continue;
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


                        // Prepare a response message
                        string responseMessage = $"{clientMessage}";
                        byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);

                        // Send the response back to the client
                        if (ConnectionMethod == 4)
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
                    ClientStream.Remove(client);
                }
            }
        }
        private static Dictionary<int, Dictionary<string, object>> DecodePacket(string Packet)
        {
            Dictionary<int, Dictionary<string, object>> listtoreturn = new Dictionary<int, Dictionary<string, object>>();
            Dictionary<string,object> packet = ApplicationUtils.JsonStringToDictionary(Packet);
            foreach(string packetKey in packet.Keys)
            {
                listtoreturn.Add(int.Parse(packetKey), ApplicationUtils.JsonStringToDictionary(packet[packetKey].ToString()));
            }
            return listtoreturn;
        }

        public static async void SendToAllClients(string message)
        {
            
            byte[] responseBuffer = Encoding.UTF8.GetBytes(message);
            foreach (TcpClient i in Clients)
            {
                await ClientStream[i].WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
        }

        public static async void SendToOtherClients(string message, TcpClient client)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes(message);
            foreach (TcpClient i in Clients)
            {
                if (ConnectionIDs[i] != ConnectionIDs[client])
                {
                    await ClientStream[i].WriteAsync(responseBuffer, 0, responseBuffer.Length);
                }
            }
        }
        public static async void CloseServer()
        {
            listener.Stop();
        }


    }

    public class Packet
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ConnectionID { get; set; }
    }
}