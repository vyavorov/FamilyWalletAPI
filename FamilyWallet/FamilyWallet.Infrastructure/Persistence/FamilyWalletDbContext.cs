using FamilyWallet.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Infrastructure.Persistence
{
    public class FamilyWalletDbContext : DbContext
    {
        public FamilyWalletDbContext(DbContextOptions<FamilyWalletDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Ако потребител бъде изтрит, трие и неговите транзакции

            modelBuilder.Entity<User>()
                .HasOne(u => u.FamilyGroup)
                .WithMany(fg => fg.Members)
                .HasForeignKey(u => u.FamilyGroupId)
                .OnDelete(DeleteBehavior.SetNull); // Ако групата бъде изтрита, потребителите остават без група

            modelBuilder.Entity<FamilyGroup>().HasIndex(fg => fg.Name).IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<FamilyGroup> FamilyGroups { get; set; }
        public DbSet<Account> Accounts{ get; set; }
        public DbSet<Category> Categories{ get; set; }

    }
}
