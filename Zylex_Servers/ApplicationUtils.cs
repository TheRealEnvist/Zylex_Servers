using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
                catch (System.Text.Json.JsonException)
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
        public static string DictionaryToJson(Dictionary<string, string> dictionary)
        {
            // Serialize the dictionary to a JSON string
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            return jsonString;
        }
        public static string DictionaryToJson(Dictionary<int, string> dictionary)
        {
            // Serialize the dictionary to a JSON string
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            return jsonString;
        }
        public static string DictionaryToJson(Dictionary<object, object> dictionary)
        {
            // Serialize the dictionary to a JSON string
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            return jsonString;
        }
        public static string DictionaryToJson(Dictionary<object, Dictionary<string, string>> dictionary)
        {
            // Serialize the dictionary to a JSON string
            string jsonString = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            return jsonString;
        }

        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            // Get all properties of the object
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead) // Check if the property can be read
                {
                    dictionary.Add(property.Name, property.GetValue(obj, null));
                }
            }

            return dictionary;
        }
        public static string ListToJson<T>(List<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }
        public static List<T> JsonToList<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException(nameof(json));

            // Deserialize the JSON string back into a List of T
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}
