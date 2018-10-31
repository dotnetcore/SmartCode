using System;
using System.Collections.Generic;
using System.Text;
using SmartCode.Configuration;
using SmartCode.ETL.Entity;
using SmartSql;
using SmartSql.Batch;

namespace SmartCode.ETL
{
    public static class ProjectExtensions
    {
        public const String ETL_TASK_ID = "ETLTaskId";
        public static void SetETKTaskId(this Project project, long etlTaskId)
        {
            if (project.Paramters.ContainsKey(ETL_TASK_ID))
            {
                project.Paramters[ETL_TASK_ID] = etlTaskId;
            }
            else
            {
                project.Paramters.Add(ETL_TASK_ID, etlTaskId);
            }
        }
        public static long GetETKTaskId(this Project project)
        {
            project.Paramters.EnsureValue(ETL_TASK_ID, out long etlTaskId);
            return etlTaskId;
        }

        public const String ETL_REPOSITORY = "ETLRepository";

        public static string GetETLRepository(this Project project)
        {
            project.Paramters.Value(ETL_REPOSITORY, out string repository);
            return repository;
        }
        public const String ETL_LAST_EXTRACT = "ETLLastExtract";
        public static void SetETLLastExtract(this Project project, ETLExtract extract)
        {
            if (project.Paramters.ContainsKey(ETL_LAST_EXTRACT))
            {
                project.Paramters[ETL_LAST_EXTRACT] = extract;
            }
            else
            {
                project.Paramters.Add(ETL_LAST_EXTRACT, extract);
            }
        }
        public static ETLExtract GetETLLastExtract(this Project project)
        {
            project.Paramters.Value(ETL_LAST_EXTRACT, out ETLExtract lastExtract);
            return lastExtract;
        }

        public const String ETL_CODE = "ETLCode";

        public static string GetETLCode(this Project project)
        {
            if (project.Paramters.Value(ETL_CODE, out string etlCode))
            {
                return etlCode;
            }
            return project.ConfigPath;
        }
    }
}
