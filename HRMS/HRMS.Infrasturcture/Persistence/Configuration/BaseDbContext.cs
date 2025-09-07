using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.RegularExpressions;
using HRMS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Persistence.Configuration
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {

        }

        protected void ApplyEntityConfigurationMaster(ModelBuilder modelBuilder, Type dbContextType)
        {
            foreach (PropertyInfo propertyInfo in dbContextType.GetProperties())
            {
                if (propertyInfo.PropertyType.IsGenericType &&
                    propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    Type? entityType = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();
                    string propertyName = propertyInfo.Name;

                    if (entityType is not null)
                        ApplyEntityConfigurationM(modelBuilder, entityType, propertyName);
                }
            }
        }


        protected void ApplyEntityConfigurationM(ModelBuilder modelBuilder, Type entityType, string tableName)
        {
            // Extract the BaseEntity type and its key type                      
            Type? baseEntityType = entityType.BaseType;
            Type? keyType = baseEntityType?.GetGenericArguments().FirstOrDefault();

            // Example usage of ApplyEntityConfigurationGeneric
            Type? dcontextClass = GetType();
            MethodInfo? method = dcontextClass?.GetMethod("ApplyEntityConfiguration", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method is not null && keyType is not null)
                method.MakeGenericMethod(entityType, keyType).Invoke(this, [modelBuilder, tableName]);
        }

        protected static void ApplyEntityConfiguration<TEntity, TKey>(ModelBuilder modelBuilder, string tableName)
    where TEntity : BaseEntity<TKey>
        {
            var entity = modelBuilder.Entity<TEntity>();
            entity.ToTable(tableName).HasKey(e => e.Id);

            var keyProp = typeof(TEntity).GetProperty(nameof(BaseEntity<TKey>.Id));
            var propBuilder = entity.Property(e => e.Id);

            if (typeof(TKey) == typeof(decimal))
            {
                ApplyDecimalPrecision(propBuilder, keyProp);
            }
            else
            {
                propBuilder.HasColumnType(GetSqlType(typeof(TKey)));
            }
        }

        private static void ApplyDecimalPrecision<T>(PropertyBuilder<T> builder, PropertyInfo? prop)
        {
            var precisionAttr = prop?.GetCustomAttribute<PrecisionAttribute>();
            if (precisionAttr != null)
            {
                builder.HasPrecision(precisionAttr.Precision, precisionAttr.Scale ?? 2);
                return;
            }

            var columnAttr = prop?.GetCustomAttribute<ColumnAttribute>();
            var match = Regex.Match(columnAttr?.TypeName ?? "", @"decimal\((\d+),\s*(\d+)\)");
            if (match.Success &&
                int.TryParse(match.Groups[1].Value, out var precision) &&
                int.TryParse(match.Groups[2].Value, out var scale))
            {
                builder.HasPrecision(precision, scale);
            }
            else
            {
                builder.HasColumnType("decimal(18,2)"); // Default fallback
            }
        }

        private static string GetSqlType(Type type) => type switch
        {
            var t when t == typeof(int) => "int",
            var t when t == typeof(long) => "bigint",
            var t when t == typeof(Guid) => "uniqueidentifier",
            _ => type.Name.ToLower()
        };
    }
}
