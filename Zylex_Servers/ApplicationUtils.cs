using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Zylex_Servers
{
    public static class ApplicationUtils
    {
        public static string GetPublicIpAddress()
        {
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

        public static Dictionary<int, string> JsonStringToDictionaryIntString(string jsonString)
        {
            var dictionary = JsonConvert.DeserializeObject<Dictionary<int, string>>(jsonString);
            return dictionary;
        }

        public static bool IsValidInteger(string input)
        {
            return int.TryParse(input, out _); // Use out _ to ignore the converted value
        }

        public static void RestartApplication()
        {
            string applicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(applicationPath);
            Environment.Exit(0); // Exit with code 0 (success)
        }

        public static Dictionary<string, object> ReadJsonFileToDictionary(string filePath)
        {
            string json = File.ReadAllText(filePath);
            Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return dictionary;
        }

        public static Dictionary<string, object> ReadLargeJsonFileToDictionary(string filePath)
        {
            using (StreamReader file = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                var serializer = new JsonSerializer();
                Dictionary<string, object> dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
                return dictionary;
            }
        }

        public static Dictionary<string, object> JsonStringToDictionary(string jsonString)
        {
            using (StringReader stringReader = new StringReader(jsonString))
            using (JsonTextReader reader = new JsonTextReader(stringReader))
            {
                var serializer = new JsonSerializer();
                Dictionary<string, object> dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
                return dictionary;
            }
        }

        public static bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]")))
            {
                try
                {
                    using (StringReader stringReader = new StringReader(input))
                    using (JsonTextReader jsonReader = new JsonTextReader(stringReader))
                    {
                        var serializer = new JsonSerializer();
                        var obj = serializer.Deserialize<object>(jsonReader);
                        return obj != null;
                    }
                }
                catch (JsonReaderException)
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
            using (StringWriter stringWriter = new StringWriter())
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, dictionary);
                return stringWriter.ToString();
            }
        }

        public static string DictionaryToJson(Dictionary<string, string> dictionary)
        {
            using (StringWriter stringWriter = new StringWriter())
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, dictionary);
                return stringWriter.ToString();
            }
        }

        public static string DictionaryToJson(Dictionary<int, string> dictionary)
        {
            using (StringWriter stringWriter = new StringWriter())
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, dictionary);
                return stringWriter.ToString();
            }
        }

        public static string DictionaryToJson(Dictionary<object, object> dictionary)
        {
            using (StringWriter stringWriter = new StringWriter())
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, dictionary);
                return stringWriter.ToString();
            }
        }

        public static string DictionaryToJson(Dictionary<object, Dictionary<string, string>> dictionary)
        {
            using (StringWriter stringWriter = new StringWriter())
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, dictionary);
                return stringWriter.ToString();
            }
        }

        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead)
                {
                    dictionary.Add(property.Name, property.GetValue(obj, null));
                }
            }

            return dictionary;
        }

        public static string ListToJson<T>(List<T> list)
        {
            using (StringWriter stringWriter = new StringWriter())
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, list);
                return stringWriter.ToString();
            }
        }

        public static List<T> JsonToList<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException(nameof(json));

            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public static Dictionary<string, object> ReadJsonStreamToDictionary(Stream jsonStream)
        {
            using (StreamReader streamReader = new StreamReader(jsonStream))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<Dictionary<string, object>>(reader);
            }
        }
        public static string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        // Method to encode a string to Base64
        public static string EncodeToBase64(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        // Method to decode a Base64 string back to its original form
        public static string DecodeFromBase64(string base64Encoded)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64Encoded);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
