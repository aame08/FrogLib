using System.Text.Json;

namespace FrogLib.Server.Services
{
    public class ContentModerationService
    {
        private readonly List<string> _forbiddedWords;
        private readonly IWebHostEnvironment _env;

        public ContentModerationService(IWebHostEnvironment env)
        {
            _env = env;
            _forbiddedWords = LoadForbiddedWords();
        }

        private List<string> LoadForbiddedWords()
        {
            var filePath = Path.Combine(_env.WebRootPath, "Resources", "forbidden-words.json");
            if (!File.Exists(filePath))
            {
                return new List<string>();
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }

        public bool ContainsForbiddenWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var lowerText = text.ToLower();
            return _forbiddedWords.Any(word =>
                !string.IsNullOrEmpty(word) && lowerText.Contains(word.ToLower()));
        }
    }
}
