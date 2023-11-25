﻿using Inventory.Service.DTO.Email;

namespace Inventory.Service
{
    public interface IEmailService
    {
        public Task<bool> SendNotificationToSA(NotificationEmailRequest request);
    }
}
