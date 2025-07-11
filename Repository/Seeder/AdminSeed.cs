using Microsoft.AspNetCore.Identity;
using System;
using TalkWithAyodeji.Data.DatabaseObject;
using TalkWithAyodeji.Repository.Data;

namespace TalkWithAyodeji.Repository.Seeder.Seed
{
    public class AdminSeed : IAdminSeed
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public AdminSeed(UserManager<AppUser> userManager, IConfiguration config, ApplicationDbContext context)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
        }
        public  async Task<int> AddDefaultAdmin()
        {
            //using var scope = _serviceScopeFactory.CreateScope();
            if (_context.Users.Any())
            {
                return 0;
            }
            else
            {
                //var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var password = _config["AdminDetails:Password"];
                var admin = new AppUser()
                {
                    UserName = $"{_config["AdminDetails:Username"]}",
                    Email = $"{_config["AdminDetails:Email"]}",
                    NormalizedEmail = ($"{_config["AdminDetails:Email"]}").ToUpper(),
                    NormalizedUserName = ($"{_config["AdminDetails:Username"]}").ToUpper(),
                    EmailConfirmed = true,
                };
                var createdUser = await _userManager.CreateAsync(admin, password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(admin, "Admin");
                    if (roleResult.Succeeded)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;

            }
            
        }
    }
}
