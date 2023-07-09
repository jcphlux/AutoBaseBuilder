using CsvHelper.Configuration;
using System.Globalization;

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