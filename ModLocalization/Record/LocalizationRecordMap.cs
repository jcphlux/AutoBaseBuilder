using System.Globalization;
using CsvHelper.Configuration;

namespace ModLocalization.Record
{
    public sealed class LocalizationRecordMap : ClassMap<LocalizationRecord>
    {
        public LocalizationRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
        }
    }
}
