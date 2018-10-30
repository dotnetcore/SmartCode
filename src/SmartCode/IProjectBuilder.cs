using SmartCode.Configuration;
using System;
using System.Threading.Tasks;

namespace SmartCode
{
    public class OnProjectBuildStartupEventArgs : EventArgs
    {
        public Project Project { get; set; }
    }
    public delegate Task OnProjectBuildStartupHandler(object sender, OnProjectBuildStartupEventArgs eventArgs);
    public class OnProjectBuildSucceedEventArgs : EventArgs
    {
        public Project Project { get; set; }
    }
    public delegate Task OnProjectBuildSucceedHandler(object sender, OnProjectBuildSucceedEventArgs eventArgs);

    public class OnProjectBuildFailedEventArgs : EventArgs
    {
        public Project Project { get; set; }
        public BuildContext Context { get; set; }
        public Exception ErrorException { get; set; }
    }
    public delegate Task OnProjectBuildFailedHandler(object sender, OnProjectBuildFailedEventArgs eventArgs);

    public interface IProjectBuilder
    {
        event OnProjectBuildStartupHandler OnStartup;
        event OnProjectBuildSucceedHandler OnSucceed;
        event OnProjectBuildFailedHandler OnFailed;
        Task Build();
    }
}
