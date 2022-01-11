using Microsoft.Extensions.DependencyInjection;
using SmartCode.TemplateEngine.Impl;
using SmartCode.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartCode.ETL.TransformEngine
{
    public class RazorTransformEngine : ITransformEngine
    {
        private const String SCRIPT = "Script";
        public async Task Transform(BuildContext context)
        {
            if (context.Build.Parameters.Value(SCRIPT, out string script) && !String.IsNullOrEmpty(script))
            {
                await RazorCoreHelper.CompileAndRunAsync(script, context);
            }
        }

        public bool Initialized { get; private set; }
        public string Name { get; private set; } = "Razor";
        private string _root = AppPath.Relative("TransformScripts");

        public void Initialize(IDictionary<string, object> parameters)
        {
            Initialized = true;
            if (parameters != null)
            {
                if (parameters.Value( "Name", out string name))
                {
                    Name = name;
                }
                if (parameters.Value( "Root", out string root))
                {
                    _root = root;
                }
            }
        }

        public Task<string> Render(BuildContext context)
        {
            return RazorCoreHelper.CompileAndRunAsync(
                context.Build.TemplateEngine.FullPath,
                context
            );
        }

    }
}
