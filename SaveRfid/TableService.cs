using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace RfidCreateAuth;

public class TableService : ITableService
{
    private readonly CloudTableWrapper _table;
    protected const string TABLE_REFERENCE = "rfidtags";

    public TableService()
    {
        var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable(@"StorageConnection"));
        var tableClient = storageAccount.CreateCloudTableClient();
        _table = new CloudTableWrapper(tableClient.GetTableReference(TABLE_REFERENCE));
    }

    public async Task<bool> TagExists(string tag)
    {
        // Retrieve the tag from storage
        var retrieveOperation = TableOperation.Retrieve<RfidTag>(nameof(RfidTag), tag);
        var retrieveResult = await _table.ExecuteAsync(retrieveOperation);

        return retrieveResult.Result is not null;
    }

    public async Task<bool> TagInsert(string tag)
    {
        if (await TagExists(tag))
        {
            return false;
        }

        // Create a new tag and insert it into storage
        var rfidTag = new RfidTag(tag);
        var inputOperation = TableOperation.Insert(rfidTag);
        var insertResult = await _table.ExecuteAsync(inputOperation);

        return (HttpStatusCode)insertResult.HttpStatusCode == HttpStatusCode.NoContent;
    }
}