﻿namespace Inventory.Model.Entity
{
    public class Category : AuditLog
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
