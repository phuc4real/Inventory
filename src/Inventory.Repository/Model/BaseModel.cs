using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public IdentityUser? CreatedBy { get; set; }

        public DateTime LastModified { get; set; }
        public string? LastModifiedById { get; set; }
        [ForeignKey(nameof(LastModifiedById))]
        public IdentityUser? LastModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

    }
}
