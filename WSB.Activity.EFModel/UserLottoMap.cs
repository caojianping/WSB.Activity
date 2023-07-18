namespace WSB.Activity.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserLottoMap")]
    public partial class UserLottoMap
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Number { get; set; }

        public DateTime LottoTime { get; set; }

        public bool Status { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
