using GLMS.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GLMS.API.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.EnsureCreatedAsync();

            // Create roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Client"))
                await roleManager.CreateAsync(new IdentityRole("Client"));

            // Create admin user
            var adminEmail = "admin@techmove.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    UserRole = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create sample client user
            var clientEmail = "client@techmove.com";
            var clientUser = await userManager.FindByEmailAsync(clientEmail);
            if (clientUser == null)
            {
                clientUser = new ApplicationUser
                {
                    UserName = clientEmail,
                    Email = clientEmail,
                    FirstName = "Demo",
                    LastName = "Client",
                    UserRole = "Client",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                await userManager.CreateAsync(clientUser, "Client@123");
                await userManager.AddToRoleAsync(clientUser, "Client");
            }

            // Create sample contracts
            if (!context.Contracts.Any())
            {
                context.Contracts.AddRange(
                    new Contract
                    {
                        ClientId = 1,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddYears(1),
                        Status = ContractStatus.Active,
                        ServiceLevel = ServiceLevel.Standard
                    },
                    new Contract
                    {
                        ClientId = 1,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddMonths(6),
                        Status = ContractStatus.Draft,
                        ServiceLevel = ServiceLevel.Premium
                    },
                    new Contract
                    {
                        ClientId = 1,
                        StartDate = DateTime.Today.AddMonths(-3),
                        EndDate = DateTime.Today.AddMonths(3),
                        Status = ContractStatus.Active,
                        ServiceLevel = ServiceLevel.Hazardous
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}