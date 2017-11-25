using System.Threading.Tasks;

namespace Repository.Common.Android.Threading
{
    public interface IYielder
    {
        Task Yield();
    }
}
