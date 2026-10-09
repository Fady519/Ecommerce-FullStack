namespace E_commerce.Domain.Entities.IdentityModule
{
    public class Address
    {

        public int Id { get; set; }

        public string City { get; set; } = default!;

        public string Street { get; set; } = default!;

        public string Country { get; set; } = default!;

        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        // RS 
        public ApplicationUser User { get; set; } = default!;

        public string UserId { get; set; } = default!;


    }
}