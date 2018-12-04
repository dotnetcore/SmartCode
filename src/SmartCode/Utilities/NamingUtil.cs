using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public class NamingUtil
    {
        public static String CamelCase(string phrase)
        {
            string firstChar = phrase.Substring(0, 1).ToLower();
            return firstChar + phrase.Substring(1);
        }
        public static String PascalCase(string phrase)
        {
            string firstChar = phrase.Substring(0, 1).ToUpper();
            return firstChar + phrase.Substring(1);
        }
    }
}
