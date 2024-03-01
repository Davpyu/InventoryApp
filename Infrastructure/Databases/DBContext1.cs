using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Models
{
    public partial class DBContext1 : DbContext
    {
        public DBContext1()
        {
        }

        public DBContext1(DbContextOptions<DBContext1> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }


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
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Base && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    ((Base)entityEntry.Entity).UpdatedAt = DateTime.Now;
                }

                if (entityEntry.State == EntityState.Added)
                {
                    ((Base)entityEntry.Entity).CreatedAt = DateTime.Now;
                }
            }

            return base.SaveChanges();
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
