using System;

namespace ModLocalization.Record
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LanguageCodeAttribute : Attribute
    {
        public string Code { get; }

        public LanguageCodeAttribute(string code)
        {
            Code = code;
        }
    }
}
