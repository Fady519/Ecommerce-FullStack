using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entites.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Persistence.IdentityData.DataSeed
{
    public class IdentityDataInitalizer : IDataInitalizer
    {
        private readonly UserManager<ApplicationUset> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataInitalizer> _logger;

        public IdentityDataInitalizer(UserManager<ApplicationUset> userManager, RoleManager<IdentityRole> roleManager ,
            ILogger<IdentityDataInitalizer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitalizeAsync()
        {
            try
            {

                if(!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admins"));
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmins"));

                }

                if(!_userManager.Users.Any())
                {
                    var User01 = new ApplicationUset()
                    {
                        DisplayName = "Fady Kaiser",
                        UserName = "Fady Kaiser",
                        Email = "fadykaiser2004@gmail.com",
                        PhoneNumber = "1234567890",
                    };
                    var User02 = new ApplicationUset()
                    {
                        DisplayName = "Beshoy Kaiser",
                        UserName = "Beshoy Kaiser",
                        Email = "beshoykaiser2004@gmail.com",
                        PhoneNumber = "1234567800",
                    };

                    await _userManager.CreateAsync(User01 , "P@ssw@rd");
                    await _userManager.CreateAsync(User02, "P@ssw@rd");
                    await _userManager.AddToRoleAsync(User01, "Admin");
                    await _userManager.AddToRoleAsync(User02, "Admin");
                }

            }

            catch (Exception ex)
            {
                _logger.LogError($"Error While Seeding Identity Database : Message = {ex.Message}");
            }

        }
    }
}
