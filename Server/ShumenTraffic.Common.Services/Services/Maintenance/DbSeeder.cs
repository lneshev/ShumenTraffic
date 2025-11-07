using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Configuration;
using ShumenTraffic.Common.Core.Constants.Security;
using ShumenTraffic.Persistence.DbContexts;
using System;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Maintenance
{
    public class DbSeeder
    {
        public static async Task SeedAppDbAsync(AppDbContext appDbContext)
        {
            var userManager = appDbContext.GetService<UserManager<IdentityUser>>();
            var usersConfig = appDbContext.GetService<IOptions<UsersConfiguration>>().Value;
            var utcNow = DateTimeOffset.UtcNow;

            #region Admin user
            var adminEmail = usersConfig.SuperAdmin.Email;
            var adminPassword = usersConfig.SuperAdmin.Password;
            var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
            if (adminUser == null)
            {
                var id = Guid.NewGuid().ToString();
                adminUser = new IdentityUser()
                {
                    Id = id,
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false
                };

                var identityResult = userManager.CreateAsync(adminUser).Result;
                if (!identityResult.Succeeded)
                {
                    throw new BusinessException("The admin user could not be created.");
                }
            }

            var isPasswordValid = userManager.CheckPasswordAsync(adminUser, adminPassword).Result;
            if (!isPasswordValid)
            {
                var token = userManager.GeneratePasswordResetTokenAsync(adminUser).Result;
                var result = userManager.ResetPasswordAsync(adminUser, token, adminPassword).Result;
                if (!result.Succeeded)
                {
                    throw new BusinessException("The admin user's password could not be set.");
                }
            }
            #endregion

            #region Admin role
            IdentityRole adminRole = await CreateRole(appDbContext, utcNow, adminUser, RoleConstants.SuperAdminRoleName);
            #endregion

            #region Attach admin role to admin user
            var isAdminUserInAdminRole = userManager.IsInRoleAsync(adminUser, adminRole.Name).Result;
            if (!isAdminUserInAdminRole)
            {
                var roleAttachResult = userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;
                if (!roleAttachResult.Succeeded)
                {
                    throw new BusinessException("The admin role could not be attached to admin user.");
                }
            }
            #endregion

            // Save changes
            await appDbContext.SaveChangesAsync();
        }

        private static async Task<IdentityRole> CreateRole(AppDbContext appDbContext, DateTimeOffset utcNow, IdentityUser adminUser, string roleName)
        {
            var role = await appDbContext.Set<IdentityRole>().FirstOrDefaultAsync(x => x.Name == roleName);
            if (role == null)
            {
                role = new IdentityRole()
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                await appDbContext.Set<IdentityRole>().AddAsync(role);
                await appDbContext.SaveChangesAsync();
            }

            return role;
        }
    }
}
