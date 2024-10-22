﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Models
{
    public partial class IamDBContext(DbContextOptions<IamDBContext> options) : DbContext(options)
    {
        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties<string>()
            .HaveMaxLength(256);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            GenerateUuid<Role>(modelBuilder, "Id");
            SoftDelete<Role>(modelBuilder);
            GenerateUuid<User>(modelBuilder, "Id");
            SoftDelete<User>(modelBuilder);
            GenerateUuid<Permission>(modelBuilder, "Id");
            SoftDelete<Permission>(modelBuilder);
            GenerateUuid<UserRole>(modelBuilder, "Id");
            SoftDelete<UserRole>(modelBuilder);
            GenerateUuid<RolePermission>(modelBuilder, "Id");
            SoftDelete<RolePermission>(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public override int SaveChanges()
        {
            var currentTime = DateTime.Now;

            var entries = ChangeTracker
                .Entries<Base>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = currentTime;
                    entry.Property(nameof(entity.CreatedAt)).IsModified = false;
                }
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.Now;

            var entries = ChangeTracker
                .Entries<Base>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = currentTime;
                    entry.Property(nameof(entity.CreatedAt)).IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        /*=================================== Service Support ===========================================*/

        private void GenerateUuid<T>(ModelBuilder modelBuilder, string column) where T : class
        {
            modelBuilder.Entity<T>()
                .HasIndex(CreateExpression<T>(column));

            modelBuilder.Entity<T>()
                .Property(CreateExpression<T>(column))
                .HasDefaultValueSql("NEWID()");
        }


        private void SetUniqueColumn<T>(ModelBuilder modelBuilder, string column) where T : class
        {
            modelBuilder.Entity<T>()
                 .HasIndex(CreateExpression<T>(column))
                .IsUnique();

        }

        private void SoftDelete<T>(ModelBuilder modelBuilder) where T : class
        {
            modelBuilder.Entity<T>()
                .HasQueryFilter(u => EF.Property<DateTime?>(u, "DeletedAt") == null);
        }

        private static Expression<Func<T, object>> CreateExpression<T>(string uuid) where T : class
        {
            var type = typeof(T);
            var property = type.GetProperty(uuid);
            var parameter = Expression.Parameter(type);
            var access = Expression.Property(parameter, property);
            var convert = Expression.Convert(access, typeof(object));
            var function = Expression.Lambda<Func<T, object>>(convert, parameter);

            return function;
        }
    }
}
