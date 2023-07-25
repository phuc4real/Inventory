namespace Inventory.Core.ViewModel
{
    public class AppUser
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class AppUserDetail : AppUser
    {
        public Permission? Permission { get; set; }
    }
}
