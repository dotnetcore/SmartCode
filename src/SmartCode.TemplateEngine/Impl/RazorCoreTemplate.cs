using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RazorEngineCore;

namespace SmartCode.TemplateEngine.Impl {

    public class RazorCoreTemplate<T> : RazorEngineTemplateBase<T> {

        public string ViewPath { get; set; }

        public string Include(string subview, BuildContext context) {
            var viewDirectory = Path.GetDirectoryName(ViewPath);
            var subviewPath = Path.Combine(viewDirectory, subview);
            return RazorCoreHelper.CompileAndRun(subviewPath, context);
        }

        public string NewLine() {
            return Environment.NewLine;
        }

        public String NewLine(string appendStr) {
            return Environment.NewLine + appendStr;
        }

        public String PadLeft(int totalWidth) {
            return String.Empty.PadLeft(totalWidth);
        }
        public String PadRight(int totalWidth) {
            return String.Empty.PadRight(totalWidth);
        }

    }

}
