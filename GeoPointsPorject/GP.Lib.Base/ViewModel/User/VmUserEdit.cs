using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GP.Lib.Base.ViewModel.Base;

namespace GP.Lib.Base.ViewModel.User
{
    public class VmUserEdit: VmBase
    {
        /// </summary>
        public string Email { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
