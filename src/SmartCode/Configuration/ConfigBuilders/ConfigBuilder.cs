using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Configuration.ConfigBuilders
{
    public abstract class ConfigBuilder : IConfigBuilder
    {
        protected Project Project { get; set; }
        public abstract Project Build();

        protected void InitDefault()
        {
            if (Project.Output != null)
            {
                if (String.IsNullOrEmpty(Project.Output.Type))
                {
                    Project.Output.Type = "File";
                }
                if (Project.Output?.Mode == CreateMode.None)
                {
                    Project.Output.Mode = CreateMode.Incre;
                }
            }

            foreach (var buildTask in Project.BuildTasks.Values)
            {
                if (buildTask.Output != null)
                {
                    if (String.IsNullOrEmpty(buildTask.Output.Type))
                    {
                        buildTask.Output.Type = Project.Output.Type;
                    }
                    if (buildTask.Output.Mode == CreateMode.None)
                    {
                        buildTask.Output.Mode = Project.Output.Mode;
                    }
                }
                if (String.IsNullOrEmpty(buildTask.TemplateEngine))
                {
                    buildTask.TemplateEngine = Project.TemplateEngine;
                }
                if (buildTask.NamingConverter == null)
                {
                    buildTask.NamingConverter = NamingConverter.Defalut;
                }
                else
                {
                    if (buildTask.NamingConverter.Table == null)
                    {
                        buildTask.NamingConverter.Table = TokenizerMapConverter.Default;
                    }
                    if (buildTask.NamingConverter.View == null)
                    {
                        buildTask.NamingConverter.View = TokenizerMapConverter.Default;
                    }
                    if (buildTask.NamingConverter.Column == null)
                    {
                        buildTask.NamingConverter.Column = TokenizerMapConverter.Default;
                    }
                }
            }
        }
    }
}
