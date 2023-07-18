namespace WSB.Activity.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Chance")]
    public partial class Chance
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ChanceType { get; set; }

        public int RuleType { get; set; }

        public int Count { get; set; }

        public bool Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
