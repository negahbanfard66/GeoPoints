using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GP.Lib.Base.ViewModel.User
{
    /// <summary>
    /// View model required to create a new user
    /// </summary>
    public class VmUserAdd : VmUserEdit
    {
        /// <summary>
        /// Users login information
        /// </summary>
        public VmUserLogin Login { get; set; }


        /// <summary>
        /// Validates the User login and User edit that this class inherits from
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var loginErrors = Login.Validate(validationContext);
            foreach (var error in loginErrors) yield return error;

            var baseErrors = base.Validate(validationContext);
            foreach (var error in baseErrors) yield return error;
        }
    }
}