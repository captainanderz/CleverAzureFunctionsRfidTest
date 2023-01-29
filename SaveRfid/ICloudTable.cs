using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace RfidCreateAuth;

public interface ICloudTable
{
    Task<TableResult> ExecuteAsync(TableOperation operation);
}