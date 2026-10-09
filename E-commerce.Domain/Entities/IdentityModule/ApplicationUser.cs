using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Entities.IdentityModule
{
    public class ApplicationUser : IdentityUser
    {

        public string DisplayName { get; set; } = default!;

        public Address? Address { get; set; }

        /// <summary>When the account was created (used by the admin dashboard: new customers per month).</summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
