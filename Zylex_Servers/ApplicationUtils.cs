using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zylex_Servers
{
    public static class ApplicationUtils
    {
        public static string GetPublicIpAddress()
        {
            // Call the async method synchronously
            return GetPublicIpAddressAsync().GetAwaiter().GetResult();
        }

        public static async Task<string> GetPublicIpAddressAsync()
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
        public static bool IsValidInteger(string input)
        {
            return int.TryParse(input, out _); // Use out _ to ignore the converted value
        }

        public static void RestartApplication()
        {
            // Get the path of the current executable
            string applicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            // Start a new instance of the application
            Process.Start(applicationPath);

            // Close the current instance
            Environment.Exit(0); // Exit with code 0 (success)
        }

        public static Dictionary<string, object> ReadJsonFileToDictionary(string filePath)
        {
            // Read the JSON file as a string
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON string into a dictionary
            Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            return dictionary;
        }
    }
}
