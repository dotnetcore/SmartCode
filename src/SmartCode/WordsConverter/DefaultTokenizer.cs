using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode.WordsConverter
{
    /// <summary>
    /// 默认分词器
    /// </summary>
    public class DefaultTokenizer : ITokenizer
    {
        public const string IGNORE_PREFIX_KEY = nameof(IgnorePrefix);
        public const string DELIMITER_KEY = nameof(Delimiter);
        /// <summary>
        /// 忽略前缀
        /// </summary>
        public String IgnorePrefix { get; set; }
        /// <summary>
        /// 分隔符
        /// </summary>
        public String Delimiter { get; set; }
        public IEnumerable<string> Segment(string phrase)
        {
            if (!String.IsNullOrEmpty(IgnorePrefix) && phrase.StartsWith(IgnorePrefix))
            {
                phrase = phrase.Substring(IgnorePrefix.Length);
            }
            if (!String.IsNullOrEmpty(Delimiter))
            {
                return phrase.Split(Delimiter.ToCharArray());
            }
            return new string[] { phrase };
        }

        public void Initialize(IDictionary<string, String> paramters)
        {
            if (paramters != null)
            {
                if (paramters.TryGetValue(IGNORE_PREFIX_KEY, out string ignorePre))
                {
                    IgnorePrefix = ignorePre;
                }
                if (paramters.TryGetValue(DELIMITER_KEY, out string delimiter))
                {
                    Delimiter = delimiter;
                }
            }
        }
    }
}
