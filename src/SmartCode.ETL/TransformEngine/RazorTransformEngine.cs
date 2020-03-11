using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using SmartCode.TemplateEngine.Impl;
using SmartCode.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Encodings.Web;
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
                using (var serviceScope = _scopeFactory.CreateScope())
                {
                    var helper = serviceScope.ServiceProvider.GetRequiredService<OfficialRazorViewToStringRenderer>();
                    await helper.RenderViewToStringAsync(script, context);
                }
            }
        }

        public bool Initialized { get; private set; }
        public string Name { get; private set; } = "Razor";
        private string _root = AppPath.Relative("TransformScripts");
        private IServiceScopeFactory _scopeFactory;
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
            InitializeServices();
        }

        public Task<string> Render(BuildContext context)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var helper = serviceScope.ServiceProvider.GetRequiredService<OfficialRazorViewToStringRenderer>();
                return helper.RenderViewToStringAsync(context.Build.TemplateEngine.FullPath, context);
            }
        }

        private void InitializeServices()
        {
            var services = ConfigureDefaultServices();
            var serviceProvider = services.BuildServiceProvider();
            _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }
        private IServiceCollection ConfigureDefaultServices()
        {
            var services = new ServiceCollection();
            IFileProvider fileProvider = new PhysicalFileProvider(_root);
            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly().GetName().Name,
                WebRootFileProvider = fileProvider,
            });
            services.AddSingleton<HtmlEncoder>(NullHtmlEncoder.Default);
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(fileProvider);
            });
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddTransient<OfficialRazorViewToStringRenderer>();
            return services;
        }
    }
}
