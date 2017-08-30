using System.Threading.Tasks;

namespace Repository.Editor.Android.Threading
{
    public interface IYielder
    {
        Task YieldAsync();
    }
}