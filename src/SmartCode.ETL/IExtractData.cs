using System.Threading.Tasks;

namespace SmartCode.ETL
{
    public interface IExtractData : IPlugin
    {
        int Total { get; }
        int BulkSize { get; }
        int Offset { get; }
        Task Run();
    }
}