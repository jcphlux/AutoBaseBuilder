using System.Globalization;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Google.Cloud.Translation.V2;
using ModLocalization.Record;

namespace ModLocalization
{
    public class TranslationUpdater
    {      
        public bool UpdateTranslations(string csvFile)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                Delimiter = ",",
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => args.Header.ToLower(),        
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            var records = new List<LocalizationRecord>();
            var hasUpdates = false;

            if (!File.Exists(csvFile))
            {
                records.Add(new LocalizationRecord());
                hasUpdates = true;
            }
            else
            {
                using (var reader = File.OpenText(csvFile))
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
                        var languageValue = property.GetValue(record) as string;
                        if (!string.IsNullOrEmpty(languageValue))
                        {
                            continue;
                        }
                        var languageCode = attribute.Code;
                        var translatedText = GetTranslatedText(record.English, languageCode, records);
                        property.SetValue(record, translatedText);
                        hasUpdates = true;
                    }
                }
            }

            if (!hasUpdates)
            {
                return false;
            }

            using (var writer = new StreamWriter(csvFile))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<LocalizationRecordMap>();
                csv.WriteRecords(records);
            }

            return true;
        }

        private string GetTranslatedText(string text, string languageCode, List<LocalizationRecord> records)
        {
            // Check if the translation already exists in any record for the given language code
            foreach (var record in records)
            {
                var languageProperty = record.GetType().GetProperty(languageCode);
                if (languageProperty == null)
                {
                    continue;
                }
                var existingTranslation = languageProperty.GetValue(record) as string;
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

        private string TranslateText(string text, string sourceLanguage, string targetLanguageCode)
        {
            TranslationClient client = TranslationClient.Create();

            TranslationResult result = client.TranslateText(text, targetLanguageCode, sourceLanguage);

            return result.TranslatedText;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Run(args);
        }

        public static void Run(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a file path as a command-line argument.");
                return;
            }

            string csvFile = args[0];
            
            var translationUpdater = new TranslationUpdater();

            if (translationUpdater.UpdateTranslations(csvFile))
            {
                Console.WriteLine("Translations updated.");
            }
            else
            {
                Console.WriteLine("No updates found.");
            }
        }
    }
}
