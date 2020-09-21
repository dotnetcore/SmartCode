using System.IO;
using System.Threading.Tasks;
using RazorEngineCore;

namespace SmartCode.TemplateEngine.Impl {

    public static class RazorCoreHelper {

        private static IRazorEngine engine;

        public static void Initialize() {
            engine = new RazorEngine();
        }

        public static Task<string> CompileAndRunAsync(
            string viewPath,
            BuildContext context
        ) {
            if (!File.Exists(viewPath)) {
                throw new FileNotFoundException($"Razor file {viewPath} does not exists!");
            }
            var template = engine.Compile<RazorCoreTemplate<BuildContext>>(
                File.ReadAllText(viewPath),
                builder => {
                    builder.AddAssemblyReferenceByName("SmartCode");
                    builder.AddAssemblyReferenceByName("SmartCode.App");
                    builder.AddAssemblyReferenceByName("SmartCode.Db");
                    builder.AddAssemblyReferenceByName("SmartCode.ETL");
                    builder.AddAssemblyReferenceByName("SmartCode.Generator");
                    builder.AddAssemblyReferenceByName("SmartCode.TemplateEngine");
                }
            );
            return template.RunAsync(instance => {
                instance.ViewPath = viewPath;
                instance.Model = context;
            });
        }

        public static string CompileAndRun(
            string viewPath,
            BuildContext context
        ) {
            var task = CompileAndRunAsync(viewPath, context);
            task.Wait();
            return task.Result;
        }
    }

}
