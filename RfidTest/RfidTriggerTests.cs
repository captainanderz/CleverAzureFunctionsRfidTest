using Microsoft.AspNetCore.Http;
using Moq;
using RfidCreateAuth;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace RfidTest
{
    public class RfidTriggerTests
    {
        private readonly Mock<ILogger> _log;
        private readonly Mock<ITableService> _tableService;
        private readonly RfidTrigger _rfidTrigger;
        protected const string RfidTag = "E200 3411 FAF2 1210";

        public RfidTriggerTests()
        {
            _log = new Mock<ILogger>();
            _tableService = new Mock<ITableService>();
            _rfidTrigger = new RfidTrigger(_tableService.Object);
        }

        [Fact]
        public async Task RunCreate_WhenTagDoesntExists_ReturnsOkRequest()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { tag = RfidTag }))));
            _tableService.Setup(t => t.TagInsert(RfidTag)).ReturnsAsync(true);

            // Act
            var result = await _rfidTrigger.RunCreate(request.Object, _log.Object);

            // Assert
            _tableService.Verify(t => t.TagInsert(RfidTag), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RunCreate_WhenTagAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { tag = RfidTag }))));

            // Act
            var result = await _rfidTrigger.RunCreate(request.Object, _log.Object);

            // Assert
            _tableService.Verify(t => t.TagInsert(RfidTag), Times.Once);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RunCreate_WhenRequestBodyIsNull_ReturnsBadRequest()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            //request.Setup(r => r.Body);
            _tableService.Setup(t => t.TagInsert(RfidTag)).ReturnsAsync(true);

            // Act
            var result = await _rfidTrigger.RunCreate(request.Object, _log.Object);

            // Assert
            _tableService.Verify(t => t.TagInsert(RfidTag), Times.Never);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RunCreate_WhenTagIsEmpty_ReturnsBadRequest()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { tag = string.Empty }))));
            _tableService.Setup(t => t.TagInsert(RfidTag)).ReturnsAsync(true);

            // Act
            var result = await _rfidTrigger.RunCreate(request.Object, _log.Object);

            // Assert
            _tableService.Verify(t => t.TagInsert(RfidTag), Times.Never);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RunAuthentication_WhenTagExists_ReturnsOkRequest()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Query["tag"]).Returns(RfidTag);
            _tableService.Setup(t => t.TagExists(RfidTag)).ReturnsAsync(true);

            // Act
            var result = await _rfidTrigger.RunAuthentication(request.Object, _log.Object);

            // Assert
            _tableService.Verify(t => t.TagExists(RfidTag), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RunAuthentication_WhenTagDoesntExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Query["tag"]).Returns(RfidTag);

            // Act
            var result = await _rfidTrigger.RunAuthentication(request.Object, _log.Object);

            // Assert
            _tableService.Verify(t => t.TagExists(RfidTag), Times.Once);

            var okObjectResult = result as OkObjectResult;
            var value = okObjectResult?.Value;
            Assert.False((bool)value);
        }

        [Fact]
        public async Task RunAuthentication_WhenTagIsEmpty_ReturnsBadRequest()
        {
            // Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Query["tag"]).Returns("");

            // Act
            var result = await _rfidTrigger.RunAuthentication(request.Object, _log.Object);

            // Assert
            _tableService.Verify(t => t.TagExists(RfidTag), Times.Never);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
