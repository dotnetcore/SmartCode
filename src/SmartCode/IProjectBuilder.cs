using SmartCode.Configuration;
using System;
using System.Threading.Tasks;

namespace SmartCode
{
    public interface IProjectBuilder
    {
        Task Build();
    }
}
