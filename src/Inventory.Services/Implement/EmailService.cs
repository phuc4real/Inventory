using Inventory.Core.Configurations;
using Inventory.Service.DTO.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Inventory.Service.Implement
{
    public class EmailService : IEmailService
    {
        #region Ctor & Field
        private readonly EmailConfig _config;

        public EmailService(IOptionsSnapshot<EmailConfig> config)
        {
            _config = config.Value;
        }

        #endregion

        #region Method

        public async Task SendEmail(EmailSenderRequest request)
        {
            var email = CreateEmail(request);

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_config.SmtpServer, _config.SmtpPort, true);
                smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                await smtp.AuthenticateAsync(_config.Email, _config.Key);
                await smtp.SendAsync(email);
            }
            catch
            {
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
                smtp.Dispose();
            }
        }

        #endregion

        #region Private 

        private MimeMessage CreateEmail(EmailSenderRequest request)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config.Sender, _config.Email));
            email.To.AddRange(request.Mails);
            email.Subject = request.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = GenerateHtmlBody(request.Body)
            };

            email.Body = builder.ToMessageBody();

            return email;
        }

        private static string GenerateHtmlBody(EmailBodyData data)
        {
            var isDev = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.InvariantCultureIgnoreCase);
            var feHost = isDev ? "http://localhost:4200" : "https://lhphuc-inventory.netlify.app";
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src\\Shared\\Inventory.Core\\Template\\", "EmailTemplate.html");
            var html = File.ReadAllText(templatePath);


            var baseInfoLeft = "<p style=\"font-size: 11px; font-family: Ubuntu, Helvetica, Arial; text-align: left;\"><span style=\"font-size: 16px;\">";
            var baseInfoRight = "</span></p>";
            var baseButtonPart1 = "<a href=\"";
            var baseButtonPart2 = "\" style=\"display: inline-block; background: #3F51B5; color: #000000; font-family: Ubuntu, Helvetica, Arial, sans-serif, Helvetica, Arial, sans-serif; font-size: 15px; font-weight: normal; line-height: 100%; margin: 0; text-decoration: none; text-transform: none; padding: 10px 30px; mso-padding-alt: 0px; border-radius: none;\" target=\"_blank\"> <span>Go to ";
            var baseButtonPart3 = "</span></a>";

            var info = "";
            var name = "";
            var link = "";
            var button = "";

            if (data.IsTicket)
            {
                name = "ticket";
                link = feHost + "/ticket/entry/" + data.InfoId;
                info += baseInfoLeft + "Ticket: No.#" + data.InfoId + baseInfoRight;
                info += baseInfoLeft + "Ticket Type: " + data.TicketType + baseInfoRight;
                info += baseInfoLeft + "Title: " + data.Title + baseInfoRight;
            }
            else
            {
                name = "order";
                link = feHost + "/order/entry/" + data.InfoId;
                info += baseInfoLeft + "Order: No.#" + data.InfoId + baseInfoRight;
            }

            info += baseInfoLeft + "Created By: " + data.InfoCreatedBy + baseInfoRight;
            info += baseInfoLeft + "Created At: " + data.InfoCreatedAt + baseInfoRight;
            info += baseInfoLeft + "Description: " + data.Description + baseInfoRight;

            button += baseButtonPart1 + link + baseButtonPart2 + name + baseButtonPart3;

            html = html.Replace("{user}", data.UserName);
            html = html.Replace("{name}", name);
            html = html.Replace("{info}", info);
            html = html.Replace("{link}", button);

            return html;
        }

        #endregion
    }
}
