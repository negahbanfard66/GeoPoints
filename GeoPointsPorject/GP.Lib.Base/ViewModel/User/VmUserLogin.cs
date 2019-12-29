using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GP.Lib.Base.ViewModel.Base;

namespace GP.Lib.Base.ViewModel.User
{
    public class VmUserLogin:VmBase
    {
        /// <summary>
        /// Login email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Users password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Re-enter password
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Checks user has filled in all fields of model and that passwords match
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email))
                yield return new ValidationResult("Email cannot be Empty, null or Whitespace", new[] { nameof(Email) });

            if (string.IsNullOrWhiteSpace(Password))
                yield return new ValidationResult("Password cannot be Empty, null or Whitespace", new[] { nameof(Password) });

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                yield return new ValidationResult("Cannot be null, empty or whitespace", new[] { nameof(ConfirmPassword) });

            if (Password != ConfirmPassword)
                yield return new ValidationResult("Passwords do not match", new[] { nameof(Password) });
        }
    }
}
