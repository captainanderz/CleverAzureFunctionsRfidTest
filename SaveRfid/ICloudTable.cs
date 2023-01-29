using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace RfidCreateAuth;

/// <summary>
/// Used for mockable CloudTable
/// </summary>
public interface ICloudTable
{
    Task<TableResult> ExecuteAsync(TableOperation operation);
}