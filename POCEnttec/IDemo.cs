using System.Threading;
using System.Threading.Tasks;

namespace POCEnttec
{
    public interface IDemo
    {
        Task Demo(CancellationToken token);
    }
}