namespace WSB.Activity.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OpenId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nickname { get; set; }

        [Required]
        [StringLength(200)]
        public string Avatar { get; set; }

        public int RedpacketsChanceCount { get; set; }

        public int LottoChanceCount { get; set; }

        public int RedpacketsTotalNumber { get; set; }

        public int LottoTotalNumber { get; set; }

        public int ExchangeIntegral { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
