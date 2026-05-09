using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Entites.IdentityModule
{
    public class ApplicationUset : IdentityUser
    {

        public string DisplayName { get; set; } = default!;

        public Address? Address { get; set; }

    }
}
