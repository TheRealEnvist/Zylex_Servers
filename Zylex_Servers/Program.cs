using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zylex_Servers
{
    internal class Program
    {
        public static byte ServerType;
        public static byte GameEngineType;
        public static int Port;
        public static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("--Server Installer--");
            Console.WriteLine(" ");
            Console.WriteLine("What kind of server would you like?");
            Console.WriteLine(" ");
            Console.WriteLine("1. Game Server");
            Console.WriteLine("2. Generic Server");
            Console.WriteLine(" ");
            Console.WriteLine("______________________");
            Console.WriteLine("  ");
            Console.Write("Server Type: ");
            string type = Console.ReadLine();
            if(Byte.Parse(type) > 2 || Byte.Parse(type) < 1)
            {
                Console.Write("Invalid server type.. Exiting Installer...");
                return;
            }
            ServerType = Byte.Parse(type);

            if(ServerType == 1)
            {
                Console.Clear();
                Console.WriteLine("--Server Installer--");
                Console.WriteLine(" ");
                Console.WriteLine("What engine is this running on?");
                Console.WriteLine(" ");
                Console.WriteLine("1. Unity");
                Console.WriteLine("2. Unreal Engine");
                Console.WriteLine("3. Other");
                Console.WriteLine("  ");
                Console.WriteLine("______________________");
                Console.WriteLine(" ");
                Console.Write("Engine Type: ");
                type = Console.ReadLine();
                if (Byte.Parse(type) > 3 || Byte.Parse(type) < 1)
                {
                    Console.Write("Invalid engine type.. Exiting Installer...");
                    return;
                }
                GameEngineType = Byte.Parse(type);
            }

            Console.Clear();
            Console.WriteLine("--Server Installer--");
            Console.WriteLine(" ");
            Console.WriteLine("Installing...");
            Console.WriteLine(" ");
            Console.WriteLine("________LOGS________");
            Console.WriteLine(" ");
            if(!Directory.Exists(appPath + "StoredData"))
            {
                Directory.CreateDirectory(appPath + "StoredData");
                Console.WriteLine("Created folder \"StoredData\" at");
                Console.WriteLine(appPath);
                Console.WriteLine(" ");
            }
            
            if (!Directory.Exists(appPath + "StoredData/Database"))
            {
                Directory.CreateDirectory(appPath + "StoredData/Database");
                Console.WriteLine("Created folder \"Database\" at");
                Console.WriteLine(appPath + "StoredData\\");
                Console.WriteLine(" ");
            }
            
            if (ServerType == 1)
            {
                if (!Directory.Exists(appPath + "StoredData/GameData"))
                {
                    Directory.CreateDirectory(appPath + "StoredData/GameData");
                    Console.WriteLine("Created folder \"GameData\" at");
                    Console.WriteLine(appPath + "StoredData\\");
                    Console.WriteLine(" ");
                }
                
                if (!Directory.Exists(appPath + "StoredData/GameData/SavedGameState"))
                {
                    Directory.CreateDirectory(appPath + "StoredData/GameData/SavedGameState");
                    Console.WriteLine("Created folder \"SavedGameState\" at");
                    Console.WriteLine(appPath + "StoredData\\GameData\\");
                    Console.WriteLine(" ");
                }
                
                if (!Directory.Exists(appPath + "StoredData/GameData/GameFile"))
                {
                    Directory.CreateDirectory(appPath + "StoredData/GameData/GameFile");
                    Console.WriteLine("Created folder \"GameFile\" at");
                    Console.WriteLine(appPath + "StoredData\\GameData\\");
                    Console.WriteLine(" ");
                }
                

            }

            Console.Clear();
            Console.WriteLine("--Server Setup--");
            Console.WriteLine(" ");
            Console.WriteLine("What port would you like the server to run on? Leave blank for deafult port.");
            Console.WriteLine(" ");
            Console.WriteLine("______________________");
            Console.WriteLine(" ");
            Console.Write(GetPublicIpAddress() + ":");

            type = Console.ReadLine();
            if (IsValidInteger(type))
            {
                Port = Convert.ToInt32(type);
            }
            else
            {
                Console.Write("Invalid port.. Exiting Installer...");
                return;
            }
            return;
        }

        static string GetPublicIpAddress()
        {
            // Call the async method synchronously
            return GetPublicIpAddressAsync().GetAwaiter().GetResult();
        }

        static async Task<string> GetPublicIpAddressAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://ipinfo.io/ip");
                    response.EnsureSuccessStatusCode();
                    string ip = await response.Content.ReadAsStringAsync();
                    return ip.Trim();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving IP address: {ex.Message}");
                    return "Unknown IP";
                }
            }
        }
        static bool IsValidInteger(string input)
        {
            return int.TryParse(input, out _); // Use out _ to ignore the converted value
        }
    }
}