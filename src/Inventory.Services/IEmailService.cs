using Inventory.Service.DTO.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service
{
    public interface IEmailService
    {
        public Task Send(EmailSenderRequest email);
    }
}
