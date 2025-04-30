using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using sys.Models;

namespace sys
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            ConfigureAuth(app);

            // ← pass 'app' into the seeder
            CreateRolesAndUsers(app);
        }

        private void CreateRolesAndUsers(IAppBuilder app)
        {
            // 1) Spin up EF context + stores
            var dbContext = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(dbContext);
            var roleStore = new RoleStore<IdentityRole>(dbContext);

            // 2) Build managers
            var userMgr = new UserManager<ApplicationUser>(userStore);
            var roleMgr = new RoleManager<IdentityRole>(roleStore);

            // 3) Register the token provider so ResetPasswordAsync works
            var dp = app.GetDataProtectionProvider();
            if (dp != null)
            {
                userMgr.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dp.Create("ASP.NET Identity"));
            }

            // 4) Constants for our default admin
            const string AdminRole = "Admin";
            const string AdminEmail = "tynoemushy@gmail.com";
            const string AdminPwd = "Admin@123";

            // 5) Ensure the Admin role exists
            if (!roleMgr.RoleExists(AdminRole))
            {
                roleMgr.Create(new IdentityRole(AdminRole));
            }

            // 6) Find or create the admin user
            var admin = userMgr.FindByEmail(AdminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true
                };
                var createRes = userMgr.Create(admin, AdminPwd);
                if (createRes.Succeeded)
                {
                    userMgr.AddToRole(admin.Id, AdminRole);
                }
            }
            else
            {
                // a) Ensure they're in the Admin role
                if (!userMgr.IsInRole(admin.Id, AdminRole))
                    userMgr.AddToRole(admin.Id, AdminRole);

                // b) Reset their password to your known default
                var token = userMgr.GeneratePasswordResetToken(admin.Id);
                userMgr.ResetPassword(admin.Id, token, AdminPwd);

                // c) Confirm email if not already
                if (!admin.EmailConfirmed)
                {
                    admin.EmailConfirmed = true;
                    userMgr.Update(admin);
                }
            }
        }
    }
}
