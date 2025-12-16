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
    internal class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.Property(x => x.Name)
            .HasMaxLength(200);

            builder.HasOne(c => c.TypeNavigation)
                .WithMany(t => t.Conversations)
                .HasForeignKey(c => c.Type)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Participants)
                .WithOne(p => p.Conversation)
                .HasForeignKey(p => p.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // For fast sorting in conversation list
            builder.HasIndex(x => x.UpdatedDate);
        }
    }
}
