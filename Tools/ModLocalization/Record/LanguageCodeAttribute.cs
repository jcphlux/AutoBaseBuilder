namespace ModLocalization.Record
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LanguageCodeAttribute : Attribute
    {
        public LanguageCodeAttribute(string code)
        {
            Code = code;
        }

        public string Code { get; }
    }
}