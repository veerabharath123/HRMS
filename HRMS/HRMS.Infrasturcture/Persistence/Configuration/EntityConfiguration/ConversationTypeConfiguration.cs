using HRMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Persistence.Configuration.EntityConfiguration
{
    public class ConversationTypeConfiguration : IEntityTypeConfiguration<ConversationType>
    {
        public void Configure(EntityTypeBuilder<ConversationType> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
