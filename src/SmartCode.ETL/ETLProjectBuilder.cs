using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;

namespace SmartCode.ETL
{
    public class ETLProjectBuilder : IProjectBuilder
    {
        private readonly Project _project;
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<ETLProjectBuilder> _logger;


        public ETLProjectBuilder(
            Project project
            , IPluginManager pluginManager
            , ILogger<ETLProjectBuilder> logger)
        {
            _project = project;
            _pluginManager = pluginManager;
            _logger = logger;
        }

        public Task Build()
        {
            var extractData = _pluginManager.Resolve<IExtractData>(_project.DataSource.Name);
            return extractData.Run();
        }
    }
}