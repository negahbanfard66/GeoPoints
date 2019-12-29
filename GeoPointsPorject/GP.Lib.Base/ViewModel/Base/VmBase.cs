using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GP.Lib.Base.ViewModel.Base
{
    public abstract class VmBase : IValidatableObject
    {
        public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
