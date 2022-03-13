using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Generator.Entity;
using SmartCode.TemplateEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCode.Generator.BuildTasks
{
    public class TableBuildTask : AbstractDbBuildTask
    {
        public const String IGNORENOPKTABLE_KEY = nameof(IgnoreNoPKTable);
        public bool IgnoreNoPKTable { get; set; }
        private readonly ILogger<TableBuildTask> _logger;
        private readonly IPluginManager _pluginManager;

        public TableBuildTask(IPluginManager pluginManager
            , ILogger<TableBuildTask> logger) : base("Table", logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }

        public override async Task Build(BuildContext context)
        {
            var filterTables = FilterTable(context.GetDataSource<ITableSource>().Tables, context.BuildKey,
                context.Build);
            context.SetCurrentAllTable(filterTables);

            var nameConverter = _pluginManager.Resolve<INamingConverter>();
            var templateEngine = _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name);
            var output = _pluginManager.Resolve<IOutput>(context.Build.Output.Type);

            var tasks = filterTables.Select(table =>
            {
                BuildContext newContext = new BuildContext();
                newContext.Project = context.Project;
                newContext.DataSource = context.DataSource;
                newContext.PluginManager = context.PluginManager;
                newContext.BuildKey = context.BuildKey;
                newContext.Build = context.Build;
                newContext.Result = context.Result;
                newContext.Output = context.Output;
                newContext.SetCurrentTable(table);

                return Task.Factory.StartNew(BuildTable,(newContext, nameConverter, templateEngine, output), TaskCreationOptions.LongRunning);
            }).ToArray();

            await Task.WhenAll(tasks);
        }

        private async void BuildTable(object obj)
        {
            var p=((BuildContext context, INamingConverter nameConverter, ITemplateEngine templateEngine, IOutput output)) obj;
            var table = p.context.GetCurrentTable();
            _logger.LogInformation($"BuildTable:{table.Name} Start!");
            p.nameConverter.Convert(p.context);
            p.context.Result = await p.templateEngine.Render(p.context);
            await p.output.Output(p.context);
            _logger.LogInformation($"BuildTable:{table.Name} End!");
        }

        public override void Initialize(IDictionary<string, object> parameters)
        {
            base.Initialize(parameters);
        }
    }
}