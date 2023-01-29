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

        [FunctionName("Create")]
        public async Task<IActionResult> RunCreate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Parse the request body for the tag
            if (req.Body == null) return new BadRequestObjectResult("Request body was empty");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string tag = data?.tag;

            // Validate the tag
            if (string.IsNullOrWhiteSpace(tag))
            {
                return new BadRequestObjectResult("The property 'Tag' was empty");
            }
            
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