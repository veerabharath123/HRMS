using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Common.Class;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HRMS.Infrastructure.Email
{
    public class MailkitServices : IEmailServices
    {
        private readonly EmailConfigDto _defaultConfig;
        public MailkitServices(IOptions<EmailConfigDto> config)
        {
            _defaultConfig = config.Value;
        }
        public Task SendMailAsync(EmailMessageDto message)
        {
            return SendMailAsync(message, _defaultConfig);
        }

        public async Task SendMailAsync(EmailMessageDto request, EmailConfigDto config)
        {
            using var client = new SmtpClient();

            var message = CreateMimeMessage(request, config);
            await client.ConnectAsync(config.SmtpServer, config.Port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(config.Username, config.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        private static MimeMessage CreateMimeMessage(EmailMessageDto request, EmailConfigDto config)
        {
            var multipartBody = new Multipart("mixed")
            {
                new TextPart("html")
                {
                    Text = request.Body,
                    ContentTransferEncoding = ContentEncoding.QuotedPrintable,
                },
            };

            AddAttachments(multipartBody, request.Attachments);

            var message = new MimeMessage
            {
                Subject = request.Subject,
                From = { new MailboxAddress(config.FromName, config.FromAddress) },
                Body = multipartBody
            };

            AddRecipients(message, request);

            return message;
        }
        private static void AddRecipients(MimeMessage message, EmailMessageDto request)
        {
            foreach (var recipient in request.To)
                message.To.Add(new MailboxAddress(recipient.DisplayName, recipient.EmailAddress));

            foreach (var cc in request.Cc)
                message.Cc.Add(new MailboxAddress(cc.DisplayName, cc.EmailAddress));

            foreach (var bcc in request.Bcc)
                message.Bcc.Add(new MailboxAddress(bcc.DisplayName, bcc.EmailAddress));
        }
        private static void AddAttachments(Multipart multipartBody, List<EmailAttachmentDto> attachments)
        {
            if (attachments.Count == 0) return;

            var body = new BodyBuilder();

            foreach (var attachment in attachments)
            {
                body.Attachments.Add(
                    attachment.FileDisplayName,
                    attachment.FileContent,
                    ContentType.Parse(attachment.FileContentType)
                );
                multipartBody.Add(body.Attachments.Last());
            }
        }
    }
}
