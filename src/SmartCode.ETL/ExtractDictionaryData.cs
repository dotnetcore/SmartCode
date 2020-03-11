using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartSql;

namespace SmartCode.ETL
{
    public class ExtractDictionaryData : AbstractExtractData
    {
        public ExtractDictionaryData(Project project, ILoggerFactory loggerFactory,
            ILogger<ExtractData> logger,
            IPluginManager pluginManager) : base(project, loggerFactory, logger, pluginManager)
        {
        }

        public IList<IDictionary<string, object>> TransformData { get; set; }

        public override string Name => "ExtractDictionary";

        protected override async Task LoadData(RequestContext requestContext)
        {
            TransformData = await SqlMapper.QueryAsync<IDictionary<string, object>>(requestContext);
        }

        public override int GetCount()
        {
            return TransformData.Count;
        }

        public override long GetMaxId(string pkColumn)
        {
            return TransformData.AsParallel().Max(dr => Convert.ToInt64(dr[pkColumn]));
        }

        public override DateTime GetMaxModifyTime(string modifyTime)
        {
            return TransformData.AsParallel().Max(dr => Convert.ToDateTime(dr[modifyTime]));
        }
    }
}