using HRMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Persistence.Configuration.EntityConfiguration
{
    internal class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
    {
        private const string UQ_MESSAGE_EMPLOYEE = "UQ_MessageStatus_Message_Employee";
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.HasIndex(ms => new { ms.MessageId, ms.EmployeeId })
            .IsUnique();

            builder.HasOne(ms => ms.Employee)
                .WithMany()
                .HasForeignKey(ms => ms.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
