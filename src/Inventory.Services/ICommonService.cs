﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service
{
    public interface ICommonService
    {
        Task<(string, string)> GetAuditUserData(string createdBy, string updatedBy);
    }
}
