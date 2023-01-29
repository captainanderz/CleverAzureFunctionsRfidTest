using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RfidCreateAuth;

[assembly: FunctionsStartup(typeof(Startup))]
namespace RfidCreateAuth;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<ITableService>(s => new TableService());
    }
}