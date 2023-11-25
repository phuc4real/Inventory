using Inventory.Core.Configurations;
using Inventory.Core.Constants;
using Inventory.Core.Template;
using Inventory.Repository;
using Inventory.Service.DTO.Email;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Inventory.Service.Implement
{
    public class EmailService : IEmailService
    {
        #region Ctor & Field
        private readonly EmailConfig _config;
        private readonly IRepoWrapper _repoWrapper;

        public EmailService(IOptionsSnapshot<EmailConfig> config, IRepoWrapper repoWrapper)
        {
            _config = config.Value;
            _repoWrapper = repoWrapper;
        }

        #endregion

        #region Method

        public async Task<bool> SendNotificationToSA(NotificationEmailRequest request)
        {
            var saList = (from role in _repoWrapper.Role.Where(x => x.Name == InventoryRoles.SuperAdmin)
                          join userRole in _repoWrapper.UserRole
                          on role.Id equals userRole.RoleId
                          join user in _repoWrapper.User
                          on userRole.UserId equals user.Id
                          select user
                         ).ToList();

            saList.ForEach(x => request.SendTo(x.FirstName + " " + x.LastName, x.Email));

            var email = CreateEmail(request);
            var isSuccess = true;
            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_config.SmtpServer, _config.SmtpPort, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_config.Email, _config.Key);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                isSuccess = false;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
                smtp.Dispose();
            }
            return isSuccess;
        }

        #endregion

        #region Private 

        private MimeMessage CreateEmail(NotificationEmailRequest request)
        {

            var builder = new BodyBuilder
            {
                HtmlBody = GenerateHtmlBody(request.Body)
            };

            var email = new MimeMessage
            {
                Subject = request.Subject,
                MessageId = "support@inventory",
                Sender = new MailboxAddress(_config.Sender, _config.Email),
                Body = builder.ToMessageBody()
            };

            email.From.Add(new MailboxAddress(_config.Sender, _config.Email));
            email.To.AddRange(request.Mails);

            return email;
        }

        private static string GenerateHtmlBody(EmailBodyData data)
        {

            var isDev = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.InvariantCultureIgnoreCase);
            var feHost = isDev ? "http://localhost:4200" : "https://lhphuc-inventory.netlify.app";
            var html = EmailTemplate.Get();


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
                link = feHost + "/ticket/entry/" + data.RecordId;
                info += baseInfoLeft + "Ticket: No.#" + data.InfoId + baseInfoRight;
                info += baseInfoLeft + "Ticket Type: " + data.TicketType + baseInfoRight;
                info += baseInfoLeft + "Title: " + data.Title + baseInfoRight;
            }
            else
            {
                name = "order";
                link = feHost + "/order/entry/" + data.RecordId;
                info += baseInfoLeft + "Order: No.#" + data.InfoId + baseInfoRight;
            }

            info += baseInfoLeft + "Created By: " + data.InfoCreatedBy + baseInfoRight;
            info += baseInfoLeft + "Created At: " + data.InfoCreatedAt + baseInfoRight;
            info += baseInfoLeft + "Description: " + data.Description + baseInfoRight;

            button += baseButtonPart1 + link + baseButtonPart2 + name + baseButtonPart3;

            html = html.Replace("{name}", name);
            html = html.Replace("{info}", info);
            html = html.Replace("{link}", button);

            return html;
        }

        #endregion
    }
}
