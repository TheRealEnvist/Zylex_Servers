using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

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
            // Deserialize the JSON string directly into a Dictionary<string, object>
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
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

        public static string ReadCompleteStringFromStream(NetworkStream stream)
        {
            // Create a memory stream to hold the received data
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[8192]; // Buffer size for reading data in chunks
                int bytesRead;

                // Read until there's no more data coming from the stream
                while (stream.CanRead && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                // Convert the memory stream to a UTF-8 encoded string
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        // Use this function to read the complete Base64 string from the stream first
        public static string DecodeFromBase64(string base64Encoded)
        {
            try
            {
                // Check if the received string is not null or empty
                if (string.IsNullOrEmpty(base64Encoded))
                {
                    throw new ArgumentException("The provided Base64 string is null or empty.");
                }

                // Handle potential missing padding
                base64Encoded = FixBase64Padding(base64Encoded);

                // Decode the Base64 string
                byte[] base64EncodedBytes = Convert.FromBase64String(base64Encoded);

                // Return the decoded string
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Base64 string format error: {ex.Message}");
                return null; // Or handle the error as needed
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during Base64 decoding: {ex.Message}");
                return null;
            }
        }

        // Helper method to fix missing padding in Base64 strings
        private static string FixBase64Padding(string base64)
        {
            // Calculate the number of padding characters required
            int padding = 4 - (base64.Length % 4);

            // If padding is required (and not 4), add '=' to the end
            if (padding > 0 && padding < 4)
            {
                base64 += new string('=', padding);
            }

            return base64;
        }

        public static string EncodeToHex(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            StringBuilder hexBuilder = new StringBuilder(plainTextBytes.Length * 2);
            foreach (byte b in plainTextBytes)
            {
                hexBuilder.AppendFormat("{0:x2}", b); // Format each byte as hexadecimal
            }
            return hexBuilder.ToString();
        }

        // Decode a Hexadecimal string
        public static string DecodeFromHex(string hexEncoded)
        {
            byte[] bytes = new byte[hexEncoded.Length / 2];
            for (int i = 0; i < hexEncoded.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexEncoded.Substring(i, 2), 16); // Convert hex to byte
            }
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
