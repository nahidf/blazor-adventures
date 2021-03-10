using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace WasmAppAuth.Infrastructure
{
    public class AccountClaimsPrincipalFactoryEx : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public AccountClaimsPrincipalFactoryEx(IAccessTokenProviderAccessor accessor) : base(accessor)
        {
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);

            if (!user.Identity.IsAuthenticated)
            {
                return user;
            }

            var identity = (ClaimsIdentity)user.Identity;
            var roleClaims = identity.FindAll(identity.RoleClaimType);

            if (roleClaims == null || !roleClaims.Any())
            {
                return user;
            }

            var rolesElem = account.AdditionalProperties[identity.RoleClaimType];

            if (rolesElem is JsonElement roles)
            {
                if (roles.ValueKind == JsonValueKind.Array)
                {
                    identity.RemoveClaim(identity.FindFirst(options.RoleClaim));
                    foreach (var role in roles.EnumerateArray())
                    {
                        identity.AddClaim(new Claim(options.RoleClaim, role.GetString()));
                    }
                }
            }

            return user;
        }
    }
}
