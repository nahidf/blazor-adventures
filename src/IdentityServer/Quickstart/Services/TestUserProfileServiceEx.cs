using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Quickstart.Services
{
    /// <summary>
    /// Profile service for test users
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.IProfileService" />
    public class TestUserProfileServiceEx : TestUserProfileService
    {
        public TestUserProfileServiceEx(TestUserStore users, ILogger<TestUserProfileServiceEx> logger): base(users, logger)
        {
        }
        
        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            base.GetProfileDataAsync(context);

            var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
            if (user != null)
            {
                var roles = user.Claims.Where(c => c.Type == JwtClaimTypes.Role.ToString());
                if (roles.Any())
                {
                    context.IssuedClaims.AddRange(roles);
                }
            }

            return Task.CompletedTask;
        }

    }
}