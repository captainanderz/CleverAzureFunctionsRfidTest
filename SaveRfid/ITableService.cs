using System.Net;
using System.Threading.Tasks;

namespace RfidCreateAuth;

public interface ITableService
{
    Task<bool> TagExists(string tag);
    Task<bool> TagInsert(string tag);
}