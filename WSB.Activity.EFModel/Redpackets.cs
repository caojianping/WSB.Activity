namespace WSB.Activity.EFModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Redpackets
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Total { get; set; }

        public int ReceiveCount { get; set; }

        public int ReceiveStatus { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
