using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGModel
{
    public class LanguageModel
    {
        private string _languageKey = string.Empty;
        private string _languageName = string.Empty;
        private string _languageFile = string.Empty;

        [BaseAttribute(Name = "LanguageKey", Description = "中英文的Key")]
        public string LanguageKey
        {
            get { return _languageKey; }
            set
            {
                _languageKey = value;
            }
        }
        [BaseAttribute(Name = "languageName", Description = "语言对应的名称")]
        public string languageName
        {
            get { return _languageName; }
            set
            {
                _languageName = value;
            }
        }
        [BaseAttribute(Name = "LanguageFile", Description = "语言对应的文件")]
        public string LanguageFile
        {
            get { return _languageFile; }
            set
            {
                _languageFile = value;
            }
        }
    }

    public class FontSizeModel
    {
        private string _FontSize = string.Empty;
        private string _FontSizeName = string.Empty;
        private string _FontSizeFile = string.Empty;

        [BaseAttribute(Name = "FontSize", Description = "字体Key")]
        public string FontSize
        {
            get { return _FontSize; }
            set
            {
                _FontSize = value;
            }
        }
        [BaseAttribute(Name = "FontSizeName", Description = "字体中文名")]
        public string FontSizeName
        {
            get { return _FontSizeName; }
            set
            {
                _FontSizeName = value;
            }
        }
        [BaseAttribute(Name = "FontSizeFile", Description = "字体对应的字体文件")]
        public string FontSizeFile
        {
            get { return _FontSizeFile; }
            set
            {
                _FontSizeFile = value;
            }
        }
    }
    public class Fontsize
    {
        private string _Key { get; set; }

        [BaseAttribute(Name = "Key", Description = "语言对应的key值")]
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        private string _size { get; set; }
        [BaseAttribute(Name = "size", Description = "对应的语言value")]
        public string size
        {
            get { return _size; }
            set { _size = value; }
        }
    }

    public class Language
    {
        private string _key { get; set; }

        [BaseAttribute(Name = "KEY", Description = "语言对应的key值")]
        public string KEY
        {
            get { return _key;}
            set { _key = value; }
        }

        private string _value { get; set; }
        [BaseAttribute(Name = "Value", Description = "对应的语言value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _languagetype { get; set; }
        [BaseAttribute(Name = "LanguageType", Description = "对应的语言语言类型")]
        public string LanguageType
        {
            get { return _languagetype; }
            set { _languagetype = value; }
        }
    }
}
