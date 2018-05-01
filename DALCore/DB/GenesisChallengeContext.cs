using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DALCore.DB
{
    public partial class GenesisChallengeContext : DbContext
    {
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersPhones> UsersPhones { get; set; }

        public GenesisChallengeContext(DbContextOptions<GenesisChallengeContext> options)
: base(options)
        { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(@"Server=.;Database=GenesisChallenge;Trusted_Connection=True;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.LastLoginOn)
                    .HasColumnName("lastLoginOn")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedOn)
                    .HasColumnName("lastUpdatedOn")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnName("passwordHash");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token");
            });

            modelBuilder.Entity<UsersPhones>(entity =>
            {
                entity.HasKey(e => e.UserPhoneId);

                entity.Property(e => e.UserPhoneId).HasColumnName("userPhoneId");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("nchar(15)");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersPhones)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UsersPhones_Users");
            });
        }
    }
}
