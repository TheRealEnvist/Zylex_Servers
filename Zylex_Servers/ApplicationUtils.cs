﻿using Newtonsoft.Json;
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

        public static Dictionary<string, object> JsonStringToDictionary(string jsonString)
        {
            // Deserialize the JSON string into a Dictionary
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            return dictionary;
        }

        public static bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false; // An empty or null string cannot be a valid JSON.
            }

            input = input.Trim(); // Remove any leading or trailing whitespace.

            // Check if the input starts and ends with the common JSON delimiters.
            if ((input.StartsWith("{") && input.EndsWith("}")) || // JSON Object
                (input.StartsWith("[") && input.EndsWith("]")))   // JSON Array
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<object>(input);
                    return true;
                }
                catch (JsonException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public static string DictionaryToJson(Dictionary<string, object> dictionary)
        {
            // Serialize the dictionary to a JSON string
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            return jsonString;
        }
    }
}
