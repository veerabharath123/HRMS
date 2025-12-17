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
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.ParentMessage)
                .WithMany(m => m.Replies)
                .HasForeignKey(m => m.ParentMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.MessageStatuses)
                .WithOne(ms => ms.Message)
                .HasForeignKey(ms => ms.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Attachments)
                .WithOne(a => a.Message)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // For fast loading and sorting chat history
            builder.HasIndex(m => new { m.ConversationId, m.CreatedUtcAt });
        }
    }
}
