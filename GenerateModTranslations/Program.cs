using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Google.Cloud.Translation.V2;

namespace GenerateModTranslations
{
    public class LocalizationRecord
    {
        public string Key { get; set; } = "key";
        public string File { get; set; } = "UI";
        public string Type { get; set; } = "Menu";
        public string English { get; set; }

        [LanguageCode("de")]
        public string German { get; set; }

        [LanguageCode("zh-cn")]
        public string Schinese { get; set; }

        [LanguageCode("es")]
        public string Latam { get; set; }

        [LanguageCode("fr")]
        public string French { get; set; }

        [LanguageCode("it")]
        public string Italian { get; set; }

        [LanguageCode("ja")]
        public string Japanese { get; set; }

        [LanguageCode("pl")]
        public string Polish { get; set; }

        [LanguageCode("pt")]
        public string Brazilian { get; set; }

        [LanguageCode("ru")]
        public string Russian { get; set; }

        [LanguageCode("tr")]
        public string Turkish { get; set; }

        [LanguageCode("zh-tw")]
        public string Tchinese { get; set; }

        [LanguageCode("es")]
        public string Spanish { get; set; }

        [LanguageCode("bg")]
        public string Bulgarian { get; set; }

        [LanguageCode("cs")]
        public string Czech { get; set; }

        [LanguageCode("da")]
        public string Danish { get; set; }

        [LanguageCode("el")]
        public string Greek { get; set; }

        [LanguageCode("et")]
        public string Estonian { get; set; }

        [LanguageCode("fi")]
        public string Finnish { get; set; }

        [LanguageCode("hu")]
        public string Hungarian { get; set; }

        [LanguageCode("id")]
        public string Indonesian { get; set; }

        [LanguageCode("lt")]
        public string Lithuanian { get; set; }

        [LanguageCode("lv")]
        public string Latvian { get; set; }

        [LanguageCode("nl")]
        public string Dutch { get; set; }

        [LanguageCode("ro")]
        public string Romanian { get; set; }

        [LanguageCode("sk")]
        public string Slovak { get; set; }

        [LanguageCode("sl")]
        public string Slovenian { get; set; }

        [LanguageCode("sv")]
        public string Swedish { get; set; }
    }

    public sealed class LocalizationRecordMap : ClassMap<LocalizationRecord>
    {
        public LocalizationRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);            
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LanguageCodeAttribute : Attribute
    {
        public string Code { get; }

        public LanguageCodeAttribute(string code)
        {
            Code = code;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a file path as a command-line argument.");
                return;
            }

            string csvFile = args[0];
            UpdateTranslations(csvFile);

            Console.WriteLine("Translations updated.");
        }

        static void UpdateTranslations(string csvFile)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                Delimiter = ",",
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => args.Header.ToLower(),                
            };

            var records = new List<LocalizationRecord>();
            var hasUpdats = false;

            if (!File.Exists(csvFile))
            {
                records.Add(new LocalizationRecord());
                hasUpdats = true;
            }
            else
            {
                using (var reader = new StreamReader(csvFile))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<LocalizationRecordMap>();
                    records.AddRange(csv.GetRecords<LocalizationRecord>());
                }
                foreach (var record in records)
                {
                    var languageProperties = record.GetType().GetProperties();
                    foreach (var property in languageProperties)
                    {
                        var attribute = property.GetCustomAttribute<LanguageCodeAttribute>();
                        if (attribute == null)
                        {
                            continue;
                        }
                        var languageValue = (string)property.GetValue(record);
                        if (!string.IsNullOrEmpty(languageValue))
                        {
                            continue;
                        }
                        var languageCode = attribute.Code;
                        var translatedText = GetTranslatedText(record.English, languageCode, records);
                        property.SetValue(record, translatedText);
                        hasUpdats = true;
                    }
                }
            }


            if (!hasUpdats)
            {
                Console.WriteLine("No updates required.");
                return;
            }
            using (var writer = new StreamWriter(csvFile))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<LocalizationRecordMap>();

                csv.WriteRecords(records);
            }
        }

        static string GetTranslatedText(string text, string languageCode, List<LocalizationRecord> records)
        {
            // Check if the translation already exists in any record for the given language code
            foreach (var record in records)
            {
                var languageProperty = record.GetType().GetProperty(languageCode);
                var existingTranslation = (string)languageProperty.GetValue(record);
                if (string.IsNullOrEmpty(existingTranslation))
                {
                    continue;
                }
                // Translation already exists, return the existing value
                return existingTranslation;
            }

            // Translation does not exist, make a translation request
            return TranslateText(text, "en", languageCode);
        }

        static string TranslateText(string text, string sourceLanguage, string targetLanguageCode)
        {
            TranslationClient client = TranslationClient.Create();

            TranslationResult result = client.TranslateText(text, targetLanguageCode, sourceLanguage);

            return result.TranslatedText;
        }
    }
}
