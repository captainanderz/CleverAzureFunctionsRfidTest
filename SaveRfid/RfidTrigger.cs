using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace RfidCreateAuth
{
    public class RfidTrigger
    {
        private readonly ITableService _tableService;

        public RfidTrigger(ITableService tableService)
        {
            _tableService = tableService;
        }

        /// <summary>
        /// Creates a new tag in a storage table.
        /// It checks if the request body is empty and if the tag property in the request body is empty.
        /// If either of these conditions are true, it returns a bad request error.
        /// If the tag does not already exist in the storage table, it is inserted and the function returns a success message.
        /// If the tag already exists, it returns a bad request error.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("Create")]
        public async Task<IActionResult> RunCreate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            if (req.Body == null) return new BadRequestObjectResult("Request body was empty");

            // Parse the request body for the tag
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string tag = data?.tag;

            if (string.IsNullOrWhiteSpace(tag))
            {
                return new BadRequestObjectResult("The property 'Tag' was empty");
            }
            
            // Try and insert tag into storage table if it doesnt already exist
            var success = await _tableService.TagInsert(tag);
            if (!success)
            {
                var logText = $"The tag '{tag}' already exists";
                log.LogInformation(logText);
                return new BadRequestObjectResult(logText);
            }

            log.LogInformation($"Successfully added {tag} to storage table");
            return new OkObjectResult("RFID inserted successfully");
        }

        /// <summary>
        /// Retrieves a string value from the query string of the request, and checks it exists in a storage table.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("Authentication")]
        public async Task<IActionResult> RunAuthentication(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            var tag = req.Query["tag"];

            if (string.IsNullOrWhiteSpace(tag))
            {
                return new BadRequestObjectResult("The query 'Tag' was empty");
            }

            // If the tag already exists, return true
            return new OkObjectResult(await _tableService.TagExists(tag));
        }
    }

    public class RfidTag : TableEntity
    {
        public RfidTag(string tag)
        {
            this.PartitionKey = nameof(RfidTag);
            this.RowKey = tag;
        }

        public RfidTag() { }
    }
}