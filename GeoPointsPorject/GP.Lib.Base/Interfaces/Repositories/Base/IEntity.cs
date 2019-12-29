using System;

namespace GP.Lib.Base.Interfaces.Repositories.Base
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
        string ModifiedBy { get; set; }
    }
}
