using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SmartCode.Utilities;
using SmartCode.App;
using System.Text;
using System.Threading;
using System.IO;
using McMaster.Extensions.CommandLineUtils;

namespace SmartCode.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ThreadPool.SetMinThreads(1000, 1000);
            ThreadPool.SetMaxThreads(1000, 1000);
            await CommandLineApplication.ExecuteAsync<SmartCodeCommand>(args);
        }
    }
}
