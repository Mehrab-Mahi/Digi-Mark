using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace Infrastructure.Context;

public class DmDbContext : DbContext
{
    public DmDbContext(DbContextOptions<DmDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Set table name to snake_case
            entity.SetTableName(ToSnakeCase(entity.GetTableName()!));

            // Set column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName()!, null))!));
            }

            // Set key names to snake_case
            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()!));
            }

            // Set foreign key names to snake_case
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName()!));
            }
        }
    }

    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var stringBuilder = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
            {
                stringBuilder.Append('_');
            }
            stringBuilder.Append(char.ToLower(input[i]));
        }

        return stringBuilder.ToString();
    }

    public DbSet<User> User { get; set; }
}