using System.Globalization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.IO;
namespace HUBTSOCIAL.src.Core.Languages
{
    public class LocalizationService
    {
        private readonly string _defaultLanguage = "Messages.vn.json";
        private readonly string _englishLanguage = "Messages.en.json";

        public string GetMessage(string key)
        {
            var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            string filePath = culture == "en" ? _englishLanguage : _defaultLanguage;
            var messages = LoadMessagesFromFile(filePath);

            return messages.ContainsKey(key) ? messages[key] : string.Empty;
        }

        private Dictionary<string, string> LoadMessagesFromFile(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "src","Core","Languages", filePath);
            var jsonData = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
        }
    }

}
