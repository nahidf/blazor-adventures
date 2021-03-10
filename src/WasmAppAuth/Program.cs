using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WasmAppAuth.Infrastructure;
using WasmAppAuth.Services;

namespace WasmAppAuth
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
            builder.Services.AddHttpClient<WeatherForecastHttpClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5016");
            }).AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

            builder.Services.AddOidcAuthentication(options =>
            {
                // Configure your authentication provider options here.
                // For more information, see https://aka.ms/blazor-standalone-auth
                options.ProviderOptions.Authority = "https://localhost:5001";
                options.ProviderOptions.ClientId = "wasmappauth-client";
                options.ProviderOptions.ResponseType = "code";

                //options.ProviderOptions.DefaultScopes.Add("weather.read");

                options.UserOptions.RoleClaim = "role";
            }).AddAccountClaimsPrincipalFactory<AccountClaimsPrincipalFactoryEx>();

            builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("edit-access",
                    new AuthorizationPolicyBuilder().
                        RequireAuthenticatedUser().
                        RequireClaim("role", "Editor").
                        Build()
                    );

                config.AddPolicy("delete-access",
                    new AuthorizationPolicyBuilder().
                        RequireAuthenticatedUser().
                        RequireRole("Admin").
                        Build()
                    );                
            });

            await builder.Build().RunAsync();
        }
    }
}
