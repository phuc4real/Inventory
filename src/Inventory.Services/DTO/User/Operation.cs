using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.User
{
    public class Operation
    {
        public ItemOperation? Item { get; set; }
        public DashboardOperation? Dashboard { get; set; }
        public CategoryOperation? Category { get; set; }
        public OrderOperation? Order { get; set; }
        public TicketOperation? Ticket { get; set; }
        public ExportOperation? Export { get; set; }
        public ItemHolderOperation? ItemHolder { get; set; }
    }

    #region Item

    public class ItemOperation
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }

    #endregion

    #region Dashboard

    public class DashboardOperation
    {
        public bool CanView { get; set; }
    }

    #endregion

    #region Category

    public class CategoryOperation
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }

    #endregion

    #region Order

    public class OrderOperation
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanApproval { get; set; }
    }

    #endregion

    #region Export

    public class ExportOperation
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }

    #endregion

    #region Ticket

    public class TicketOperation
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanChangeStatus { get; set; }
        public bool CanApproval { get; set; }
    }

    #endregion

    #region ItemHolder

    public class ItemHolderOperation
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }

    #endregion
}
