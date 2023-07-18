namespace WSB.Activity.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class View_UserLottoMap
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Number { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime LottoTime { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool Status { get; set; }

        public DateTime? UpdateTime { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(100)]
        public string Nickname { get; set; }
    }
}
