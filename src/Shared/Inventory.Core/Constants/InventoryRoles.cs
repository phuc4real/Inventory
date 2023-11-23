namespace Inventory.Core.Constants
{
    public class InventoryRoles
    {
        public const string NormalUser = "Normal User";
        public const string Admin = "Administrator";
        public const string SuperAdmin = "Super Administrator";

        public const string AdminOrSuperAdmin = Admin + "," + SuperAdmin;
    }
}
