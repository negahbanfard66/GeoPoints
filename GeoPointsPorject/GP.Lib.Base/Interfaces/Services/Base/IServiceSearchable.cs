using System.Threading.Tasks;
using GP.Lib.Base.Core;
using GP.Lib.Base.ViewModel.User;

namespace GP.Lib.Base.Interfaces.Services.Base
{
    public interface IServiceSearchable<T>
    {
        Task<PaginationResult<T>> GetAllAsync(VmUserData userData, string includeProperties = null);
    }
}