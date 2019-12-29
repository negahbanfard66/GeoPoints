using System.Collections.Generic;
using System.Threading.Tasks;
using GP.Lib.Base.DataLayer;
using GP.Lib.Base.Interfaces.Services.Base;
using GP.Lib.Base.ViewModel.User;

namespace GP.Lib.Base.Interfaces.Services
{
    public interface IServiceUser : IServiceSearchable<DbUser>
    {
        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="createdBy"></param>
        /// <param name="inviteLink"></param>
        /// <returns></returns>
        Task<VmUserResult> AddAsync(VmUserAdd model, string createdBy, string inviteLink);

        /// <summary>
        /// Confirm Users Email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> ConfirmEmailAsync(string email, string token);

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<VmUserResult>> GetAsync();

        /// <summary>
        /// Find User By Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<VmUserResult> FindAsync(int userId);

        /// <summary>
        /// Update user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="vmEdit"></param>
        /// <param name="vmData"></param>
        /// <returns></returns>
        Task<VmUserResult> UpdateAsync(int userId, VmUserEdit vmEdit = null, VmUserData vmData = null);

        /// <summary>
        /// Delete User(s) by Id
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Get's Users by user type
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imagePath"></param>
        Task UpdateUserProfilePhotoAsync(int userId, string imagePath);
    }
}
