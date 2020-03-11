using System.Threading.Tasks;

namespace SmartCode.ETL
{
    public interface IExtractData : IPlugin
    {
        long Total { get; }
        int BulkSize { get; }
        int Offset { get; }
        Task Run();
    }
}