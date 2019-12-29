using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GP.Lib.Base.Interfaces.Repositories.Base;

namespace GP.Lib.Base.DataLayer.Base
{
    public abstract class Entity : IEntity
    {
        protected Entity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [Column(Order = 1)]
        public DateTime CreatedAt { get; set; }

        [Column(Order = 2)]
        public string CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Column(Order = 3)]
        public DateTime? ModifiedAt { get; set; }

        [Column(Order = 4)]
        public string ModifiedBy { get; set; }
    }
}
