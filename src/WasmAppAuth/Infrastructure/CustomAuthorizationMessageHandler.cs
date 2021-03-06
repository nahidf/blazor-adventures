using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WasmAppAuth.Infrastructure
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
            NavigationManager navigationManager)
            : base(provider, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { "https://localhost:5016" },                
                scopes: new[] { "weather.read" });
        }
    }
}
