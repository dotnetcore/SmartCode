using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SmartCode.Utilities;

namespace SmartCode.TemplateEngine.Impl {

    public class RazorCoreTemplateEngine : ITemplateEngine {

        public bool Initialized { get; private set; }
        public string Name { get; private set; } = "RazorCore";

        private string _root = AppPath.Relative("RazorTemplates");

        public void Initialize(IDictionary<string, object> parameters) {
            if (parameters != null) {
                if (parameters.Value("Name", out string name))
                {
                    Name = name;
                }
                if (parameters.Value("Root", out string root))
                {
                    _root = root;
                }
            }
            RazorCoreHelper.Initialize();
            Initialized = true;
        }

        public Task<string> Render(BuildContext context) {
            var viewPath = Path.Combine(_root, context.Build.TemplateEngine.FullPath);
            return RazorCoreHelper.CompileAndRunAsync(viewPath, context);
        }

    }

}
