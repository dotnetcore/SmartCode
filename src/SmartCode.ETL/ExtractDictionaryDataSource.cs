using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartSql;

namespace SmartCode.ETL
{
    public class ExtractDictionaryDataSource : AbstractExtractDataSource
    {
        public ExtractDictionaryDataSource(Project project, ILoggerFactory loggerFactory,
            ILogger<ExtractDataSource> logger,
            IProjectBuilder projectBuilder, IPluginManager pluginManager) : base(project, loggerFactory, logger,
            projectBuilder, pluginManager)
        {
        }

        public IList<IDictionary<string, object>> TransformData { get; set; }

        public override string Name => "ExtractDictionary";

        protected override async Task LoadData(RequestContext requestContext)
        {
            TransformData = await SqlMapper.QueryAsync<IDictionary<string, object>>(requestContext);
        }

        protected override int GetQuerySize()
        {
            return TransformData.Count;
        }

        protected override long GetMaxId(string pkColumn)
        {
            return TransformData.AsParallel().Max(dr => Convert.ToInt64(dr[pkColumn]));
        }

        protected override DateTime GetMaxModifyTime(string modifyTime)
        {
            return TransformData.AsParallel().Max(dr => Convert.ToDateTime(dr[modifyTime]));
        }
    }
}