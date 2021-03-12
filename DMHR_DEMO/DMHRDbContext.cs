using Microsoft.EntityFrameworkCore;

namespace DMHR_DEMO
{
    public partial class DMHRDbContext : DbContext
    {
        public DMHRDbContext(DbContextOptions<DMHRDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //数据库模式名称,一定要添加，否则会报错
            modelBuilder.HasDefaultSchema("DMHR"); 

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("CITY");
                entity.Property(e => e.CITY_ID).HasMaxLength(3);
                entity.Property(e => e.CITY_NAME).HasMaxLength(40);
                entity.Property(e => e.REGION_ID).HasColumnType("int(10)");
            });
        }
    }
}
