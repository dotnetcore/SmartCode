using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.App.BuildTasks
{
    public abstract class AbstractBuildTask : IBuildTask
    {
        protected readonly ILogger _logger;

        public AbstractBuildTask(string name, ILogger logger)
        {
            Name = name;
            _logger = logger;
        }

        public bool Initialized { get; protected set; }

        public string Name { get; protected set; }

        public abstract Task Build(BuildContext context);

        public virtual void Initialize(IDictionary<string, object> parameters)
        {
            Initialized = true;
        }
    }
}
