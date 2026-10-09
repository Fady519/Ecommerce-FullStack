namespace E_commerce.Domain.Entities.IdentityModule
{
    /// <summary>
    /// Role names in one place, so a typo can't create a second "Admins" role
    /// (the old seed created "Admins" but assigned "Admin", so nobody was ever an admin).
    /// </summary>
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static readonly string[] All = [Admin, Customer];
    }
}
