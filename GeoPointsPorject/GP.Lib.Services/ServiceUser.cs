using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using GP.Lib.Base.DataLayer;

using GP.Lib.Base.Interfaces.Repositories;
using GP.Lib.Base.Interfaces.Services;
using GP.Lib.Base.ViewModel.User;
using GP.Lib.Services.Base;
using GP.Lib.Services.Exceptions;
using GP.Lib.Services.Mapping;

namespace GP.Lib.Services
{
    public class ServiceUser : SearchableService<DbUser>, IServiceUser
    {
        #region Fields
        private readonly IRepositoryUser _userRepo;

        private readonly UserManager<DbUser> _userManager;

        private readonly IHostingEnvironment _env;
        private readonly RoleManager<DbRole> _roleManager;

        #endregion

        #region Ctor
        public ServiceUser(
            IRepositoryUser userRepo,
            IRepositoryTutors tutorRepo,
            UserManager<DbUser> userManager,
            RoleManager<DbRole> roleManager,
            IServiceEmailSender emailService,
            IServiceUserProfile userProfileService,
            ISieveProcessor sieveProcessor,
            IServiceAddress serviceAddress,
            IHostingEnvironment hostingEnvironment,
            IRepositoryStudent studentRepo,
            IServiceEducationLevel eduLevelService,
            IRepositoryLinkTutorSubject tutorSubjectRepo) : base(userRepo, sieveProcessor)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _tutorRepo = tutorRepo ?? throw new ArgumentNullException(nameof(tutorRepo));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
            _addressService = serviceAddress ?? throw new ArgumentNullException(nameof(serviceAddress));
            _env = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _studentRepo = studentRepo ?? throw new ArgumentNullException(nameof(studentRepo));
            _eduLevelService = eduLevelService ?? throw new ArgumentNullException(nameof(eduLevelService));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates DbUser and returns VmUserAddEdit, Biggest of our user models
        /// </summary>
        /// <param name="model"></param>
        /// <param name="createdBy"></param>
        /// <returns>
        /// User Add Edit View model
        /// </returns>
        public async Task<VmUserResult> AddAsync(VmUserAdd model, string createdBy, string inviteLink)
        {
            //Check user doesn't already exist
            var user = await _userRepo.GetFirstAsync(u => u.Email == model.Login.Email);
            if (user != null) throw new BadRequestException("E-mail already in use");

            //If there is no user with this email then lets create           
            var entity = new DbUser();
            entity.FromAddVM(model, createdBy);
            entity.SecurityStamp = Guid.NewGuid().ToString();

            var identityResult = await _userManager.CreateAsync(entity, model.Login.Password);

            if (identityResult.Succeeded)
            {
                var result = entity.ToUserResultVM();
                //Set users address
                if (model.ContactInfo?.Address != null)
                {
                    result.ContactInfo.Address = await AddAddress(entity, model.ContactInfo.Address);
                }
                if (model.Profile != null)
                {
                    var profile = await AddProfile(model.Profile, result.Id);
                    result.Profile.UpdateVmUserProfile(profile);
                }
                //SendEmail
                await SendUserConfirmationEmail(model.Login.Email, model.UserType, inviteLink);
                //Return View model of our newly created User
                //Make sure we set role
                await AddUserRole(model.UserType, entity);

                return result;
            }
            else
            {
                throw new BadRequestException(identityResult.Errors.Select(x => x.Description));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> AddUserRole( DbUser user)
        {
            var roleToFind = userType.ToString();

            var role = await _roleManager.FindByNameAsync(roleToFind);
            if (role == null) throw new BadRequestException("Invalid User Role");

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            return result.Succeeded;
        }





        #region Send confirmation email
        /// <summary>
        /// Send confirmation email, switch on User Type
        /// Note these emails do not exist yet
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userType"></param>
        /// <param name="inviteLink"></param>
        private async Task SendUserConfirmationEmail(string email, EnumUserType userType, string inviteLink)
        {
            // consider moving to a factory...
            switch (userType)
            {
                case EnumUserType.Admin:
                    //Send Admin User Confirmation Email
                    await SendEmail(email,
                        ConstantEmailTemplate.AdminEmailTitle,
                        ConstantEmailTemplate.AdminEmailTemplate,
                        inviteLink,
                        "Admin");
                    break;
                case EnumUserType.Student:
                    await SendEmail(email,
                        ConstantEmailTemplate.StudentEmailTitle,
                         ConstantEmailTemplate.StudentEmailTemplate,
                        inviteLink,
                        "Student");
                    //Send Student Confirmation email
                    break;
                case EnumUserType.Tutor:
                    await SendEmail(email,
                        ConstantEmailTemplate.TutorEmailTitle,
                        ConstantEmailTemplate.TutorEmailTemplate,
                        inviteLink,
                        "Tutor");
                    //Send Tutor email
                    break;
                case EnumUserType.Payer:
                    //Send Payee Email
                    await SendEmail(email,
                        ConstantEmailTemplate.PayeeEmailTitle,
                        ConstantEmailTemplate.PayeeEmailTemplate,
                        inviteLink,
                        "Payee");
                    break;
                case EnumUserType.Unassigned:
                default:
                    await SendEmail(email,
                        ConstantEmailTemplate.UnassignedDefaultEmailTitle,
                        ConstantEmailTemplate.UnassignedDefaultEmailTemplate,
                        inviteLink,
                        "Unassigned");
                    break;
            }
        }

        /// <summary>
        /// Gets user from Db, creates a token to validate them and then sends an email depending on the users UserType
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailTemplate"></param>
        /// <returns></returns>
        //private async Task SendEmail(string emailTo, string emailTitle,
        //    string emailTemplate, string inviteLink, string userType)
        //{
        //    var user = await _userManager.FindByEmailAsync(emailTo);
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    inviteLink += $"&token={HttpUtility.UrlEncode(token)}";
        //    await _emailService.SendEmailAsync(emailTo, emailTitle, emailTemplate, inviteLink, "Tutor time", userType);
        //}
        #endregion

        #region Confirm Email

        //public async Task<bool> ConfirmEmailAsync(string email, string token)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //        throw new BadRequestException("Sorry there was a problem verifying your e-mail");

        //    var emailResult = await _userManager.ConfirmEmailAsync(user, HttpUtility.HtmlDecode(token));

        //    if (emailResult.Succeeded)
        //    {
        //        user.Enabled = true;
        //        user.EmailConfirmed = true;
        //        await _userRepo.UpdateAsync(user);
        //        _userRepo.Save();

        //        return true;
        //    }
        //    else
        //        throw new BadRequestException(emailResult.Errors.Select(e => e.Description));

        //}
        #endregion

        #region Get / Find
        public async Task<IEnumerable<VmUserResult>> GetAsync() => await _userRepo.GetAll().Select(x => x.ToUserResultVM(null, null, null)).ToListAsync();

        public async Task<VmUserResult> FindAsync(int id)
        {
            var user = await _userRepo.FindAsync(id);
            if (user == null) throw new NotFoundException(id, "User");

            DbStudent student = null;
            var keyStageLevel = new VmEducationLevel();
            if (user.UserType == EnumUserType.Student)
            {
                student = _studentRepo.GetAllNoTracking("EducationalLevel,AdditionalNeeds").FirstOrDefault(s => s.UserId == user.Id);
                if (student.EducationalLevelId > 0 && student.EducationalLevelId != null)
                {
                    int eduId = student.EducationalLevelId ?? default(int);
                    if (eduId > 0)
                        keyStageLevel = await _eduLevelService.GetByIdAsync(eduId);
                }
            }
            if (user.UserType == EnumUserType.Payer)
            {
                return user.ToUserResultVM();
            }
            return user.ToUserResultVM(await _tutorRepo.FindWithUserIdAsync(id), student);
        }

        #endregion

        #region Update

        public async Task<VmUserResult> UpdateAsync(int userId, VmUserEdit vmEdit = null, VmUserData vmData = null)
        {
            var user = await _userRepo.FindAsync(userId);
            if (user == null)
                throw new NotFoundException(userId, "User");
            //Update the user
            if (vmEdit != null)
                user.FromEditVM(vmEdit);
            else if (vmData != null)
                user.FromDataVM(vmData);

            await _userRepo.UpdateAsync(user);
            _userRepo.Save();

            return user.ToUserResultVM(null, null, vmEdit.Profile);
        }

        #endregion

        #region Delete By Id

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepo.FindAsync(id);
            if (user == null)
                throw new NotFoundException(id, "User");

            await _userManager.DeleteAsync(user);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public async Task UpdateUserProfilePhotoAsync(int userId, string imagePath) => await _userProfileService.UpdateUserImageAsync(userId, imagePath);

        public async Task<VmUserInfo> GetUserInfo(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var userRoles = await _userManager.GetRolesAsync(user);
            var keyStageLevel = new VmEducationLevel();

            if (userRoles.Contains("Student"))
            {
                var student = _studentRepo.GetAllNoTracking("User").FirstOrDefault(s => s.User.Id == user.Id);
                if (student.EducationalLevelId > 0 && student.EducationalLevelId != null)
                {
                    int eduId = student.EducationalLevelId ?? default(int);
                    if (eduId > 0)
                        keyStageLevel = await _eduLevelService.GetByIdAsync(eduId);
                }
            }
            return user.ToUserInfoVM(userRoles, keyStageLevel);
        }

        #endregion
    }
}
