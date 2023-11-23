using MimeKit;

namespace Inventory.Service.DTO.Email
{
    public class EmailSenderRequest
    {
        public List<MailboxAddress>? Mails { get; set; }
        public string? Subject { get; set; }
        public EmailBodyData? Body { get; set; }
    }
}
