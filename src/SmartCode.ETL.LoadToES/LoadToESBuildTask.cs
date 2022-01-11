using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;
using System.Linq;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using System.Diagnostics;
using System.Collections;
using System.Data;

namespace SmartCode.ETL.LoadToES
{
    public class LoadToESBuildTask : IBuildTask
    {
        private const string COLUMN_MAPPING = "ColumnMapping";
        private const string ID_MAPPING = "Id";
        private const string HOST = "Host";
        private const string INDEX_NAME = "Index";
        private const string TYPE_NAME = "Type";
        private const string BASE_AUTH = "BaseAuth";
        private const string CERT_PATH = "Cert";
        private readonly Project _project;
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<LoadToESBuildTask> _logger;

        public bool Initialized => true;

        public string Name => "LoadToES";
        public LoadToESBuildTask(Project project
        , IPluginManager pluginManager
        , ILogger<LoadToESBuildTask> logger)
        {
            _project = project;
            _pluginManager = pluginManager;
            _logger = logger;
        }
        public async Task Build(BuildContext context)
        {
            ESOptions esOptions = InitOptions(context);
            var etlRepository = _pluginManager.Resolve<IETLTaskRepository>(_project.GetETLRepository());
            var dataSource = context.GetExtractData<ExtractDictionaryData>();
            var loadEntity = new Entity.ETLLoad
            {
                Size = 0,
                Parameters = new Dictionary<String, object>
                    {
                        { "Task","LoadToES"},
                        { "Index",esOptions.Index},
                        { "Type",esOptions.TypeName}
                    }
            };
            if (dataSource.TransformData.Count == 0)
            {
                await etlRepository.Load(_project.GetETKTaskId(), loadEntity);
                return;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            #region InitColumnMapping

            var colMapping = new Dictionary<string, string>();
            if (context.Build.Parameters.Value(COLUMN_MAPPING, out IEnumerable colMapps))
            {
                foreach (IDictionary<object, object> colMappingKV in colMapps)
                {
                    colMappingKV.EnsureValue("Column", out string colName);
                    colMappingKV.EnsureValue("Mapping", out string mapping);
                    colMapping.Add(colName, mapping);
                }
            }
            dataSource.TransformData = dataSource.TransformData.Select((dic) =>
             {
                 IDictionary<string, object> newItem = new Dictionary<string, object>();
                 foreach (KeyValuePair<string, object> item in dic)
                 {
                     var itemKey = item.Key;
                     if (colMapps != null)
                     {
                         if (colMapping.TryGetValue(itemKey, out string mapping))
                         {
                             itemKey = mapping;
                         }
                     }
                     newItem.Add(itemKey,item.Value);
                 }
                 return newItem;
             }).ToList();

            #endregion
            #region BatchInsert
            var esClient = GetElasticClient(esOptions);
            var indexExResp = await esClient.Indices.ExistsAsync(esOptions.Index);
            if (!indexExResp.Exists)
            {
                var createIndexResp = await esClient.Indices.CreateAsync(esOptions.Index);
            }
            var esSyncResp = await esClient.BulkAsync((bulkRequest) => {
                    var bulkReqDesc = bulkRequest
                        .Index(esOptions.Index);
                  // .Type(esOptions.TypeName);
                if (context.Build.Parameters.Value(ID_MAPPING, out string es_id))
                {
                    return bulkReqDesc.IndexMany(dataSource.TransformData, (bulkIdxDesc, item) =>
                    {
                        var idVal = item[es_id].ToString();
                        return bulkIdxDesc.Id(idVal);
                    }
                    );
                }
                return bulkReqDesc.IndexMany(dataSource.TransformData);
            }
            );
            if (esSyncResp.Errors || !esSyncResp.IsValid)
            {
                _logger.LogError($"ES.ERRORS:{esSyncResp.DebugInformation}");
                throw new SmartCodeException($"ES.ERRORS:{esSyncResp.DebugInformation}");
            }
            stopwatch.Stop();
            loadEntity.Size = dataSource.TransformData.Count;
            loadEntity.Taken = stopwatch.ElapsedMilliseconds;
            #endregion
            await etlRepository.Load(_project.GetETKTaskId(), loadEntity);
        }

        private static ESOptions InitOptions(BuildContext context)
        {
            context.Build.Parameters.EnsureValue(HOST, out string host);
            context.Build.Parameters.EnsureValue(INDEX_NAME, out string index_name);
            context.Build.Parameters.EnsureValue(TYPE_NAME, out string type_name);
            context.Build.Parameters.Value(BASE_AUTH, out IDictionary<object, object> baseAuth);
            baseAuth.Value("UserName", out string user_name);
            baseAuth.Value("Password", out string password);
            context.Build.Parameters.Value(CERT_PATH, out string cert);

            return new ESOptions
            {
                Host = host,
                Index = index_name,
                TypeName = type_name,
                Cert = cert,
                UserName = user_name,
                Password = password
            };
        }

        private IElasticClient GetElasticClient(ESOptions esOptions)
        {
            Uri node = new Uri(esOptions.Host);
            ConnectionSettings settings = new ConnectionSettings(node);
            settings.DefaultIndex(esOptions.Index);
            // settings.DefaultTypeName(esOptions.TypeName);
            #region Auth
            if (!String.IsNullOrEmpty(esOptions.UserName))
            {
                settings.BasicAuthentication(esOptions.UserName, esOptions.Password);
            }
            if (!String.IsNullOrEmpty(esOptions.Cert))
            {
                settings.ClientCertificate(esOptions.Cert);
            }
            #endregion
            return new ElasticClient(settings);
        }

        public void Initialize(IDictionary<string, object> parameters)
        {

        }
    }
}
