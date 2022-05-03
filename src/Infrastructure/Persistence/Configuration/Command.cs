using System;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.WebApi.Domain.Command;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.WebApi.Infrastructure.Persistence.Configuration
{
    public class CommandConfig : IEntityTypeConfiguration<Command>
    {
        public void Configure(EntityTypeBuilder<Command> builder)
        {
            builder
                .ToTable("Commands", SchemaNames.Catalog);

            builder
                .IsMultiTenant();

            builder
                .Property(b => b.Source)
                    .HasConversion(
                        v => v.ToString(),
                        v => (CommandSource)Enum.Parse(typeof(CommandSource), v));
        }
            

        
    }
}

