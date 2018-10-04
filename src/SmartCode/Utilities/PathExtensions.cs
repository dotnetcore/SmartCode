using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartCode.Utilities
{
    public class AppPath
    {
        /// <summary>
        /// 应用相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String Relative(string path)
        {
            return Path.Combine(AppContext.BaseDirectory, path);
        }
    }
}
