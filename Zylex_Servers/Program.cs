namespace Zylex_Servers
{
    internal class Program
    {
        public byte ServerType;
        public byte GameEngineType;
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
            Console.Write("Server Type: ");
            string type = Console.ReadLine();

        }
    }
}