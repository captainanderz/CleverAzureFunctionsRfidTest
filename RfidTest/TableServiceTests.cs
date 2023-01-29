using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using RfidCreateAuth;

namespace RfidTest;

public class TableServiceTests
{
    private readonly Mock<ITableService> _tableService;
    private readonly Mock<ICloudTable> _tableMock;

    public TableServiceTests()
    {
        _tableMock = new Mock<ICloudTable>();
        _tableService = new Mock<ITableService>();
    }
    
    [Theory]
    [InlineData("E200 3411 FAF2 1210", true)]
    [InlineData("",  false)]
    public async Task TableService_TagExists(string tag, bool exists)
    {
        //arrange
        var dto = new TestDto(tag, exists);
        _tableMock.Setup(t => t.ExecuteAsync(It.IsAny<TableOperation>())).ReturnsAsync(dto.TableResult);
        
        //act
        var result = await _tableService.Object.TagExists(dto.Tag);

        //assert
        Assert.Equal(result, dto.TableResult == null);
    }

    [Theory]
    [InlineData("E200 3411 FAF2 1210", true)]
    [InlineData("E200 3411 FAF2 1210", false)]
    [InlineData("", false)]
    public async void TableService_TagInsert(string tag, bool exists)
    {
        //arrange
        var dto = new TestDto(tag, exists);
        _tableMock.Setup(t => t.ExecuteAsync(It.IsAny<TableOperation>())).ReturnsAsync(dto.TableResult);

        //act
        var result = await _tableService.Object.TagInsert(dto.Tag);

        //assert
        Assert.Equal(result, dto.TableResult == null);
    }

    private class TestDto
    {
        public string Tag { get; }
        public TableResult TableResult { get; }

        public TestDto(string tag, bool exists)
        {
            Tag = tag;
            TableResult = exists
                ? new TableResult() { Result = new RfidTag(tag) }
                : new TableResult() { Result = null };
        }
    }
}
