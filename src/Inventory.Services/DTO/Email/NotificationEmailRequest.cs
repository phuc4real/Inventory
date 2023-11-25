using MimeKit;

namespace Inventory.Service.DTO.Email
{
    public class NotificationEmailRequest
    {
        public NotificationEmailRequest()
        {
            Mails = new List<MailboxAddress>();
        }
        public List<MailboxAddress>? Mails { get; set; }
        public string? Subject { get; set; }
        public EmailBodyData? Body { get; set; }
        public void SendTo(string name, string email)
        {
            Mails.Add(new MailboxAddress(name, email));
        }
    }
}
