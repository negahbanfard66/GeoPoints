using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
//using TT.Lib.Base.Enums;
using GP.Lib.Base.Interfaces.Repositories.Base;


namespace GP.Lib.Base.DataLayer
{
    public class DbUser : IdentityUser<int>, IEntity
    {
        #region IEntity implementation
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        #endregion
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}