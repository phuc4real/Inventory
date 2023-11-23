using Inventory.Core.Configurations;
using Inventory.Service.DTO.Email;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

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

        public async Task Send(EmailSenderRequest email)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_config.Url)
            };
            httpClient.DefaultRequestHeaders
                      .Authorization = new AuthenticationHeaderValue
                      ("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(_config.ApiKey)));

            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("from", _config.From),
                    new KeyValuePair<string, string>("to", email.SendTo),
                    new KeyValuePair<string, string>("subject", email.Subject),
                    new KeyValuePair<string, string>("html", email.Message)
            });

            await httpClient.PostAsync(_config.Domain, content);
        }

        #endregion
    }
}
