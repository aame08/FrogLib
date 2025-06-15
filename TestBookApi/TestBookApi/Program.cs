using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestBookApi
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            string bookTitle = "Благословение небожителей. Том 2";
            //string bookTitle = "Пиши, сокращай 2025: Как создавать сильный текст";

            Console.WriteLine("=== Данные из Google Books API ===");
            await GetBookDataFromGoogleBooksAsync(bookTitle);

            //Console.WriteLine("\n=== Данные из Open Library API ===");
            //await GetBookDataFromOpenLibraryAsync(bookTitle);

            Console.ReadKey();
        }

        private static string GetHighQualityImageUrl(string imageUrl, int zoomLevel = 3)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            // Если URL начинается с "http://", заменим на "https://"
            if (imageUrl.StartsWith("http://"))
            {
                imageUrl = "https://" + imageUrl.Substring(7);
            }

            // Теперь мы уверены, что URL начинается с "https://"
            var uriBuilder = new UriBuilder(imageUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

            // Изменяем параметр zoom на новый уровень
            query["zoom"] = zoomLevel.ToString();
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

        private static async Task GetBookDataFromGoogleBooksAsync(string title)
        {
            string apiKey = "AIzaSyBP0RdShwD1NnK47iEBl6-yCe8Lf8EAnkI";
            string requestUri = $"https://www.googleapis.com/books/v1/volumes?q={title}&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject bookData = JObject.Parse(jsonResponse);

                    var items = bookData["items"] as JArray;
                    if (items != null && items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            var volumeInfo = item["volumeInfo"];
                            if (volumeInfo != null)
                            {
                                // Проверяем, есть ли описание
                                var description = volumeInfo["description"]?.ToString();
                                if (string.IsNullOrWhiteSpace(description)) // Если описание пустое, пропускаем книгу
                                {
                                    continue;
                                }

                                Console.WriteLine("Информация о книге:");
                                foreach (JProperty field in volumeInfo.Children<JProperty>())
                                {
                                    string fieldName = field.Name;
                                    string fieldValue = field.Value.ToString();

                                    // Проверяем поле imageLinks и обрабатываем его
                                    if (fieldName == "imageLinks")
                                    {
                                        var imageLinks = JObject.Parse(fieldValue);
                                        string thumbnailUrl = imageLinks["thumbnail"]?.ToString();
                                        string highQualityUrl = GetHighQualityImageUrl(thumbnailUrl, zoomLevel: 3);

                                        Console.WriteLine($"Поле: {fieldName}, Значение: ");
                                        Console.WriteLine($"  Оригинал: {thumbnailUrl}");
                                        Console.WriteLine($"  Увеличенное: {highQualityUrl}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Поле: {fieldName}, Значение: {fieldValue}");
                                    }
                                }
                                Console.WriteLine(new string('-', 40)); // Разделитель между книгами
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Книги не найдены в Google Books.");
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка запроса к Google Books API: " + response.ReasonPhrase);
                }
            }
        }

        private static async Task GetBookDataFromOpenLibraryAsync(string title)
        {
            string baseUrl = "https://openlibrary.org/search.json?q=";
            string requestUrl = baseUrl + Uri.EscapeDataString(title);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseData);

                    var docs = json["docs"]?.ToObject<JArray>();  // Преобразуем в массив

                    if (docs != null && docs.Count > 0)
                    {
                        var firstBook = docs[0];  // Доступ к первой книге в массиве

                        foreach (var field in firstBook.Children<JProperty>())
                        {
                            string fieldName = field.Name;
                            string fieldValue = field.Value.ToString();

                            // Обработка изображения (если оно есть)
                            if (fieldName == "cover_i")
                            {
                                string coverId = fieldValue;
                                string imageUrl = $"https://covers.openlibrary.org/b/id/{coverId}-L.jpg";
                                Console.WriteLine($"URL изображения: {imageUrl}");
                            }
                            else
                            {
                                Console.WriteLine($"Поле: {fieldName}, Значение: {fieldValue}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Книга не найдена в Open Library.");
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка запроса к Open Library API: {response.StatusCode}");
                }
            }
        }
    }
}
