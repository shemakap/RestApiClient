using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestApiClient
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task Main(string[] args)
        {
            string baseUrl = "https://api.restful-api.dev/objects";

            // Step 1: Perform GET request
            await GetObjectsAsync(baseUrl);

            // Step 2: Perform POST request
            var newObject = new ApiRequest
            {
                Name = "Apple MacBook Pro 16",
                Data = new Dictionary<string, object>
                {
                    { "year", 2019 },
                    { "price", 1849.99 },
                    { "CPU model", "Intel Core i9" },
                    { "Hard disk size", "1 TB" }
                }
            };
            var createdObject = await PostObjectAsync(baseUrl, newObject);

            if (createdObject != null)
            {
                // Step 3: Perform GET request for the newly created object by ID
                await GetObjectByIdAsync(baseUrl, createdObject.Id);

                // Step 4: Perform PUT request to update the object
                var updatedObject = new ApiRequest
                {
                    Name = "Apple MacBook Pro 16",
                    Data = new Dictionary<string, object>
                    {
                        { "year", 2019 },
                        { "price", 2049.99 },
                        { "CPU model", "Intel Core i9" },
                        { "Hard disk size", "1 TB" },
                        { "color", "silver" }
                    }
                };
                var updatedApiResponse = await PutObjectAsync(baseUrl, createdObject.Id, updatedObject);

                if (updatedApiResponse != null)
                {
                    // Step 5: Perform DELETE request to delete the object
                    await DeleteObjectAsync(baseUrl, updatedApiResponse.Id);
                }
            }
        }

        public static async Task GetObjectsAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                List<ApiResponse> apiResponses = JsonSerializer.Deserialize<List<ApiResponse>>(jsonResponse);
                foreach (var item in apiResponses)
                {
                    Console.WriteLine($"ID: {item.Id}");
                    Console.WriteLine($"Name: {item.Name}");
                    if (item.Data != null)
                    {
                        foreach (var dataItem in item.Data)
                        {
                            Console.WriteLine($"{dataItem.Key}: {dataItem.Value}");
                        }
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Error: Unable to retrieve data");
            }
        }

        public static async Task<ApiResponse> PostObjectAsync(string url, ApiRequest newObject)
        {
            var json = JsonSerializer.Serialize(newObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ApiResponse createdObject = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
                Console.WriteLine("Created Object:");
                Console.WriteLine($"ID: {createdObject.Id}");
                Console.WriteLine($"Name: {createdObject.Name}");
                foreach (var dataItem in createdObject.Data)
                {
                    Console.WriteLine($"{dataItem.Key}: {dataItem.Value}");
                }
                Console.WriteLine($"CreatedAt: {createdObject.CreatedAt}");
                return createdObject;
            }
            else
            {
                Console.WriteLine("Error: Unable to create object");
                return null;
            }
        }

        public static async Task GetObjectByIdAsync(string baseUrl, string id)
        {
            string url = $"{baseUrl}/{id}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
                Console.WriteLine("Retrieved Object by ID:");
                Console.WriteLine($"ID: {apiResponse.Id}");
                Console.WriteLine($"Name: {apiResponse.Name}");
                if (apiResponse.Data != null)
                {
                    foreach (var dataItem in apiResponse.Data)
                    {
                        Console.WriteLine($"{dataItem.Key}: {dataItem.Value}");
                    }
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Error: Unable to retrieve object by ID");
            }
        }

        public static async Task<ApiResponse> PutObjectAsync(string baseUrl, string id, ApiRequest updatedObject)
        {
            string url = $"{baseUrl}/{id}";
            var json = JsonSerializer.Serialize(updatedObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
                Console.WriteLine("Updated Object:");
                Console.WriteLine($"ID: {apiResponse.Id}");
                Console.WriteLine($"Name: {apiResponse.Name}");
                if (apiResponse.Data != null)
                {
                    foreach (var dataItem in apiResponse.Data)
                    {
                        Console.WriteLine($"{dataItem.Key}: {dataItem.Value}");
                    }
                }
                Console.WriteLine($"UpdatedAt: {apiResponse.UpdatedAt}");
                Console.WriteLine();
                return apiResponse;
            }
            else
            {
                Console.WriteLine("Error: Unable to update object");
                return null;
            }
        }

        public static async Task DeleteObjectAsync(string baseUrl, string id)
        {
            string url = $"{baseUrl}/{id}";
            HttpResponseMessage response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Delete Response:");
                Console.WriteLine(jsonResponse);
            }
            else
            {
                Console.WriteLine("Error: Unable to delete object");
            }
        }
    }

    public class ApiRequest
    {
        public string Name { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    public class ApiResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
