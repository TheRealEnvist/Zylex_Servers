namespace Zylex_Servers
{
    internal class Program
    {
        public static byte ServerType;
        public static byte GameEngineType;
        public static string appPath = AppDomain.CurrentDomain.BaseDirectory;
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("--Server Installer--");
            Console.WriteLine("");
            Console.WriteLine("What kind of server would you like?");
            Console.WriteLine("");
            Console.WriteLine("1. Game Server");
            Console.WriteLine("2. Generic Server");
            Console.WriteLine("");
            Console.WriteLine("______________________");
            Console.WriteLine("");
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
                Console.WriteLine("");
                Console.WriteLine("What engine is this running on?");
                Console.WriteLine("");
                Console.WriteLine("1. Unity");
                Console.WriteLine("2. Unreal Engine");
                Console.WriteLine("3. Other");
                Console.WriteLine("");
                Console.WriteLine("______________________");
                Console.WriteLine("");
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
            Console.WriteLine("");
            Console.WriteLine("Installing...");
            Console.WriteLine("");
            Console.WriteLine("________LOGS________");
            Console.WriteLine("");
            if(!Directory.Exists(appPath + "StoredData"))
            {
                Directory.CreateDirectory(appPath + "StoredData");
                Console.WriteLine("Created folder \"StoredData\" at");
                Console.WriteLine(appPath);
                Console.WriteLine("");
            }
            
            if (!Directory.Exists(appPath + "StoredData/Database"))
            {
                Directory.CreateDirectory(appPath + "StoredData/Database");
                Console.WriteLine("Created folder \"Database\" at");
                Console.WriteLine(appPath + "StoredData\\");
                Console.WriteLine("");
            }
            
            if (ServerType == 1)
            {
                if (!Directory.Exists(appPath + "StoredData/GameData"))
                {
                    Directory.CreateDirectory(appPath + "StoredData/GameData");
                    Console.WriteLine("Created folder \"GameData\" at");
                    Console.WriteLine(appPath + "StoredData\\");
                    Console.WriteLine("");
                }
                
                if (!Directory.Exists(appPath + "StoredData/GameData/SavedGameState"))
                {
                    Directory.CreateDirectory(appPath + "StoredData/GameData/SavedGameState");
                    Console.WriteLine("Created folder \"SavedGameState\" at");
                    Console.WriteLine(appPath + "StoredData\\GameData\\");
                    Console.WriteLine("");
                }
                
                if (!Directory.Exists(appPath + "StoredData/GameData/GameFile"))
                {
                    Directory.CreateDirectory(appPath + "StoredData/GameData/GameFile");
                    Console.WriteLine("Created folder \"GameFile\" at");
                    Console.WriteLine(appPath + "StoredData\\GameData\\");
                    Console.WriteLine("");
                }
                

            }


            return;
        }
    }
}