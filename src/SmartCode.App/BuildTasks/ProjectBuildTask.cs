using Microsoft.Extensions.Logging;
using SmartCode.TemplateEngine;
using System.Threading.Tasks;

namespace SmartCode.App.BuildTasks
{
    public class ProjectBuildTask : AbstractBuildTask
    {
        private readonly IPluginManager _pluginManager;

        public ProjectBuildTask(
              IPluginManager pluginManager
            , ILogger<ProjectBuildTask> logger) : base("Project", logger)
        {
            _pluginManager = pluginManager;
        }
        public override async Task Build(BuildContext context)
        {
            context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name).Render(context);
            await _pluginManager.Resolve<IOutput>(context.Build.Output.Type).Output(context);
        }
    }
}
