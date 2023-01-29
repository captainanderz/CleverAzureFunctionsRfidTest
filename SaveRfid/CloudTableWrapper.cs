using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace RfidCreateAuth;

/// <summary>
/// This is a wrapper so CloudTable is mockable
/// </summary>
public class CloudTableWrapper : ICloudTable
{
    private readonly CloudTable _cloudTable;

    public CloudTableWrapper(CloudTable cloudTable)
    {
        _cloudTable = cloudTable;
    }

    public Task<TableResult> ExecuteAsync(TableOperation operation)
    {
        return _cloudTable.ExecuteAsync(operation);
    }

    public Task<TableResult> ExecuteAsync(TableOperation operation, TableRequestOptions requestOptions, OperationContext operationContext)
    {
        return _cloudTable.ExecuteAsync(operation, requestOptions, operationContext);
    }
}