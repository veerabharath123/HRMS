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
    internal class ConversationParticipantsConfiguration : IEntityTypeConfiguration<ConversationParticipants>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipants> builder)
        {
            builder.HasIndex(x => new { x.ConversationId, x.EmployeeId })
            .IsUnique();

            builder.HasOne(cp => cp.Employee)
                .WithMany()
                .HasForeignKey(cp => cp.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
