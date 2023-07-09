using CsvHelper.Configuration.Attributes;

namespace ModLocalization.Record
{
    public class LocalizationRecord
    {
        [Optional]
        [LanguageCode("pt")]
        public string? Brazilian { get; set; }

        [Optional]
        [LanguageCode("bg")]
        public string? Bulgarian { get; set; }

        [Optional]
        [LanguageCode("cs")]
        public string? Czech { get; set; }

        [LanguageCode("da")]
        public string? Danish { get; set; }

        [LanguageCode("nl")]
        public string? Dutch { get; set; }

        public string English { get; set; } = "English Value";

        [Optional]
        [LanguageCode("et")]
        public string? Estonian { get; set; }

        public string File { get; set; } = "UI";

        [Optional]
        [LanguageCode("fi")]
        public string? Finnish { get; set; }

        [Optional]
        [LanguageCode("fr")]
        public string? French { get; set; }

        [Optional]
        [LanguageCode("de")]
        public string? German { get; set; }

        [Optional]
        [LanguageCode("el")]
        public string? Greek { get; set; }

        [LanguageCode("hu")]
        public string? Hungarian { get; set; }

        [Optional]
        [LanguageCode("id")]
        public string? Indonesian { get; set; }

        [Optional]
        [LanguageCode("it")]
        public string? Italian { get; set; }

        [Optional]
        [LanguageCode("ja")]
        public string? Japanese { get; set; }

        public string Key { get; set; } = "key";

        [Optional]
        [LanguageCode("es")]
        public string? Latam { get; set; }

        [Optional]
        [LanguageCode("lv")]
        public string? Latvian { get; set; }

        [LanguageCode("lt")]
        public string? Lithuanian { get; set; }

        [Optional]
        [LanguageCode("pl")]
        public string? Polish { get; set; }

        [Optional]
        [LanguageCode("ro")]
        public string? Romanian { get; set; }

        [Optional]
        [LanguageCode("ru")]
        public string? Russian { get; set; }

        [Optional]
        [LanguageCode("zh-cn")]
        public string? Schinese { get; set; }

        [Optional]
        [LanguageCode("sk")]
        public string? Slovak { get; set; }

        [Optional]
        [LanguageCode("sl")]
        public string? Slovenian { get; set; }

        [LanguageCode("es")]
        public string? Spanish { get; set; }

        [Optional]
        [LanguageCode("sv")]
        public string? Swedish { get; set; }

        [Optional]
        [LanguageCode("zh-tw")]
        public string? Tchinese { get; set; }

        [Optional]
        [LanguageCode("tr")]
        public string? Turkish { get; set; }

        public string Type { get; set; } = "Menu";
    }
}