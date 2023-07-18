namespace WSB.Activity.EFModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ActivityContext : DbContext
    {
        public ActivityContext()
            : base("name=ActivityContext")
        {
        }

        public virtual DbSet<Chance> Chance { get; set; }
        public virtual DbSet<Redpackets> Redpackets { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserLottoMap> UserLottoMap { get; set; }
        public virtual DbSet<UserRedpacketsMap> UserRedpacketsMap { get; set; }
        public virtual DbSet<View_Redpackets> View_Redpackets { get; set; }
        public virtual DbSet<View_UserLottoMap> View_UserLottoMap { get; set; }
        public virtual DbSet<View_UserReceiveRedpackets> View_UserReceiveRedpackets { get; set; }
        public virtual DbSet<View_UserRedpacketsMap> View_UserRedpacketsMap { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(e => e.OpenId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Avatar)
                .IsUnicode(false);

            modelBuilder.Entity<View_Redpackets>()
                .Property(e => e.Avatar)
                .IsUnicode(false);

            modelBuilder.Entity<View_UserRedpacketsMap>()
                .Property(e => e.Avatar)
                .IsUnicode(false);
        }
    }
}
